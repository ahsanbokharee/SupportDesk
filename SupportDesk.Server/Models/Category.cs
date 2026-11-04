namespace SupportDesk.Server.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Prefix { get; set; } = "";
    public int LastNumber { get; set; }
    public string DefaultPriority { get; set; } = "Medium";
    public string Color { get; set; } = "#607d8b";
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
}