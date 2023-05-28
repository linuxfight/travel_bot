using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

#pragma warning disable CS8604
#pragma warning disable CS8602

internal class Program
{
    private static string token = Environment.GetEnvironmentVariable("TOKEN"); //get the bot token

    private static async Task Main(string[] args)
    {
        var stopEvent = new ManualResetEvent(false);
        var bot = new TelegramBotClient(token);
        bot.StartReceiving(
            HandleUpdateAsync,
            HandlePollingError,
            new ReceiverOptions()
            {
                ThrowPendingUpdates = true,
                AllowedUpdates = Array.Empty<UpdateType>()
            }
        );

        var me = await bot.GetMeAsync();
        Console.WriteLine($"Logged in as t.me/{me.Username}");
        stopEvent.WaitOne();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await OnMessage(botClient, update.Message);
                break;
            case UpdateType.CallbackQuery:
                await OnButton(botClient, update.CallbackQuery);
                break;
        }
    }

    private static async Task OnButton(ITelegramBotClient bot, CallbackQuery query)
    {
        var user = Service.GetUser(query.Message.Chat.Id);
        var travels = Service.GetTravels(user);
        var data = query.Data.Split(":");
        switch (data[0])
        {
            case "edit":
                var editTravel = travels.FirstOrDefault(x => x.Id == int.Parse(data[1]));
                await Service.ChangeUserTravel(user, editTravel);
                await Service.ChangeUserState(user, State.Name);
                await bot.SendTextMessageAsync(
                    query.Message.Chat.Id,
                    "Введите название для выезда"
                );
                break;
            case "remove":
                await Service.DeleteTravel(user, int.Parse(data[1]));
                await bot.SendTextMessageAsync(
                    query.Message.Chat.Id,
                    "Готово!"
                );
                break;
            case "addUser":
                await Service.EditTravelUser(user, int.Parse(data[1]));
                await bot.AnswerCallbackQueryAsync(
                    query.Id,
                    "Теперь вы учавствуете в выезде"
                );
                await bot.EditMessageReplyMarkupAsync(
                    query.Message.Chat.Id,
                    query.Message.MessageId,
                    TravelKeyboard(travels.FirstOrDefault(x => x.Id == int.Parse(data[1])), user)
                );
                break;
            case "removeUser":
                await Service.EditTravelUser(user, int.Parse(data[1]));
                await bot.AnswerCallbackQueryAsync(
                    query.Id,
                    "Теперь вы не учавствуете в выезде"
                );
                await bot.EditMessageReplyMarkupAsync(
                    query.Message.Chat.Id,
                    query.Message.MessageId,
                    TravelKeyboard(travels.FirstOrDefault(x => x.Id == int.Parse(data[1])), user)
                );
                break;
            default:
                var travel = travels.FirstOrDefault(x => x.Id == int.Parse(data[0]));
                await bot.SendPhotoAsync(
                    query.Message.Chat.Id,
                    new InputFileId(travel.PictureFileId),
                    caption: $"*{travel.Name}*\n" + 
                    $"{travel.Description}\n" +
                    $"Цена: {travel.Price}\n" +
                    $"Участники: {travel.People.Count}/{travel.MaxPeople}\n" +
                    $"{travel.StartTime.ToString()} - {travel.EndTime.ToString()}",
                    parseMode: ParseMode.Markdown,
                    replyMarkup: TravelKeyboard(travel, user)
                );
                break;
        }
    }

    private static async Task OnMessage(ITelegramBotClient bot, Message message)
    {
        var user = Service.GetUser(message.Chat.Id);
        switch (message.Text)
        {
            case "/start":
                if (user == null)
                {
                    var FullName = $"{message.Chat.FirstName} {message.Chat.LastName}";
                    await Service.CreateUser(FullName, message.Chat.Id);
                }
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"Привет, {message.Chat.FirstName}! Добро пожаловать в бота. Здесь ты можешь посмотреть список доступных приключений и оплатить его!",
                    replyToMessageId: message.MessageId
                );
                break;
            case "/addtravel":
                if (user.Admin)
                {
                    await Service.ChangeUserTravel(user, new Travel());
                    await Service.ChangeUserState(user, State.Name);
                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Введите название для выезда",
                        replyToMessageId: message.MessageId
                    );
                }
                break;
            case "/cancel":
                await Service.ChangeUserState(user, State.Normal);
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "Текущее действие отменено",
                    replyToMessageId: message.MessageId
                );
                break;
            case "/travels":
                var travels = Service.GetTravels(user);
                if (travels.Count == 0)
                {
                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Выездов нет",
                        replyToMessageId: message.MessageId
                    );
                }
                else
                {
                    await bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Список выездов",
                        replyToMessageId: message.MessageId,
                        replyMarkup: TravelsKeyboard(travels)
                    );
                }
                break;
            default:
                await OnEdit(user, bot, message);
                break;
        }
    }

    private static async Task OnEdit(User user, ITelegramBotClient bot, Message message)
    {
        switch (user.State)
            {
                case State.Name:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.Description);
                    await SendMessage(bot, message, "Введите описание выезда");
                    break;
                case State.Description:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.Picture);
                    await SendMessage(bot, message, "Отправьте фотографию выезда");
                    break;
                case State.Picture:
                    try {await Service.EditTravel(user, message.Photo[message.Photo.Length - 1].FileId);} catch {}
                    await Service.ChangeUserState(user, State.Price);                        
                    await SendMessage(bot, message, "Введите цену выезда");
                    break;
                case State.Price:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.MaxPeople);
                    await SendMessage(bot, message, "Введите макс. кол-во людей на выезде");                        
                    break;
                case State.MaxPeople:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.Available);
                    await SendMessage(bot, message, "Будет ли выезд доступен (по умолч. - нет)");                        
                    break;
                case State.Available:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.StartTime);
                    await SendMessage(bot, message, "Введите время начала выезда (например 22.02.2022)");
                    break;
                case State.StartTime:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.EndTime);
                    await SendMessage(bot, message, "Введите время окончания выезда");
                    break;
                case State.EndTime:
                    try {await Service.EditTravel(user, message.Text);} catch {}
                    await Service.ChangeUserState(user, State.Normal);
                    await SendMessage(bot, message, "Готово!");
                    break;
                }
    }

    private static Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {exception}");
        return null;
    }

    private static InlineKeyboardMarkup TravelKeyboard(Travel travel, User user)
    {
        var buttons = new List<InlineKeyboardButton>();
        if (travel.People.Contains(user))
            buttons.Add(InlineKeyboardButton.WithCallbackData("✅", $"removeUser:{travel.Id}"));
        else
            buttons.Add(InlineKeyboardButton.WithCallbackData("❌", $"addUser:{travel.Id}"));
        if (user.Admin){
            buttons.AddRange(
                new List<InlineKeyboardButton>(){
                    InlineKeyboardButton.WithCallbackData("✏️", $"edit:{travel.Id}"),
                    InlineKeyboardButton.WithCallbackData("🗑️", $"remove:{travel.Id}")
            });}
        return new InlineKeyboardMarkup(buttons);
    }

    private static async Task SendMessage(ITelegramBotClient bot, Message message, string text)
    {
        await bot.SendTextMessageAsync(
            message.Chat.Id,
            text,
            replyToMessageId: message.MessageId
        );
    }

    private static InlineKeyboardMarkup TravelsKeyboard(List<Travel> travels)
    {
        var buttons = new List<List<InlineKeyboardButton>>();
        foreach (var travel in travels)
            buttons.Add(new List<InlineKeyboardButton>(){InlineKeyboardButton.WithCallbackData(travel.Name, travel.Id.ToString())});
        return new InlineKeyboardMarkup(buttons);
    }
}