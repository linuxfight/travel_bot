using System.Globalization;
using System.Text.RegularExpressions;

#pragma warning disable CS8602
#pragma warning disable CS8604

public static class Service
{
    private static long AdminId = long.Parse(Environment.GetEnvironmentVariable("ADMIN_ID"));
    private static BotContext Context = new BotContext();

    private static bool useRegex(String input)
    {
        Regex regex = new Regex("(0?[1-9]|[12][0-9]|3[01])\\.(0?[1-9]|[1][0-2])\\.[0-9]+", RegexOptions.IgnoreCase);
        return regex.IsMatch(input);
    }

    public static List<Travel> GetTravels(User user)
    {
        if (user.Admin)
            return Context.Travels.ToList();
        return Context.Travels.Where(x => x.Available).ToList();
    }

    public static User? GetUser(long TelegramId)
    {
        return Context.Users.FirstOrDefault(x => x.TelegramId == TelegramId);
    }

    public static async Task CreateUser(string FullName, long TelegramId)
    {
        var user = new User()
        {
            FullName = FullName,
            TelegramId = TelegramId
        };
        if (TelegramId == AdminId)
            user.Admin = true;
        Context.Users.Add(user);
        await Context.SaveChangesAsync();
    }

    public static async Task DeleteTravel(User user, int id)
    {
        var travel = Context.Travels.FirstOrDefault(x => x.Id == id);
        if (user.Admin)
            Context.Travels.Remove(travel);
        await Context.SaveChangesAsync();
    }

    public static async Task ChangeUserState(User user, State state)
    {
        if (user.State != state)
            user.State = state;
        await Context.SaveChangesAsync();
    }

    public static async Task ChangeUserTravel(User user, Travel travel)
    {
        if (!Context.Travels.Contains(travel))
            Context.Travels.Add(travel);
        user.Travel = travel;
        await Context.SaveChangesAsync();
    }

    public static async Task EditTravelUser(User user, int id)
    {
        var travel = Context.Travels.FirstOrDefault(x => x.Id == id);
        if (travel.People.Contains(user))
            travel.People.Remove(user);
        else
            travel.People.Add(user);
        await Context.SaveChangesAsync();
    }

    public static async Task EditTravel(User user, string value)
    {
        if (user.Admin)
        {
            var travel = Context.Travels.FirstOrDefault(x => x.Id == user.Travel.Id);
            switch (user.State)
            {
                case State.Name:
                    travel.Name = value;
                    break;
                case State.Description:
                    travel.Description = value;
                    break;
                case State.Picture:
                    travel.PictureFileId = value;
                    break;
                case State.Price:
                    travel.Price = int.Parse(value);
                    break;
                case State.MaxPeople:
                    travel.MaxPeople = int.Parse(value);
                    break;
                case State.Available:
                    switch (value.ToLower())
                    {
                        case "да":
                            travel.Available = true;
                          break;
                        case "нет":
                            travel.Available = false;
                            break;
                        case "da":
                            travel.Available = true;
                          break;
                        case "net":
                            travel.Available = false;
                            break;
                        case "lf":
                            travel.Available = true;
                            break;
                        case "ytn":
                            travel.Available = false;
                            break;
                    }
                    break;
                case State.StartTime:
                    if (useRegex(value))
                        travel.StartTime = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
                case State.EndTime:
                    if (useRegex(value))
                        travel.StartTime = DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    break;
            }
            await Context.SaveChangesAsync();
        }
    }
}