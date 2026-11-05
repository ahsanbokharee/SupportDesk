namespace SupportDesk.Server.Models;

public class Ticket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string Priority { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }                  // ← foreign key column
    public Category Category { get; set; } = null!;      // ← navigation property
    public int Number { get; set; }                      // ← number within its CURRENT category 
    public List<TicketAlias> Aliases { get; set; } = new();          // ← add: keys it had before moves
}