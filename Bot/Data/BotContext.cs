using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

public class BotContext : DbContext
{
    public DbSet<User> Users {get;set;}
    public DbSet<Travel> Travels {get;set;}

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        var db = "bot.db";
        var path = dir + db;
        options.UseSqlite($"Data Source={path}");
    }
}