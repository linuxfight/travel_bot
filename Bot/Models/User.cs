using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum State
{
    Normal,
    Name,
    Description,
    Picture,
    Price,
    MaxPeople,
    Available,
    StartTime,
    EndTime
}

public class User
{
    [Key]
    public int Id {get;set;}
    public string FullName {get;set;} = "DefaultUsername";
    public bool Admin {get;set;} = false;
    public long TelegramId {get;set;}
    public State State {get; set;} = State.Normal;
    public Travel? Travel {get;set;} = null;
}