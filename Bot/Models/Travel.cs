using System.ComponentModel.DataAnnotations;

public class Travel
{
    [Key]
    public int Id {get;set;}
    public string Name {get;set;} = "Default Travel";
    public string Description {get;set;} = "NEVER GONNA GIVE YOU UP...";
    public string PictureFileId {get;set;} = "AgACAgIAAxkBAAPtZHKGK3YcwW-HHoi0s48-3ycwGusAAhXJMRssx5FLsNQSZfgBjYABAAMCAAN4AAMvBA";
    public int Price {get;set;} = 0;
    public int MaxPeople {get;set;} = 1;
    public List<User> People {get;set;} = new();
    public bool Available {get;set;} = false;
    public DateTime StartTime {get;set;} = DateTime.Now;
    public DateTime EndTime {get;set;} = DateTime.Now;
}