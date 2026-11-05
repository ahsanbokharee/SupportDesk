namespace SupportDesk.Server.Models;

public class TicketAlias
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; } = null!;
    public int CategoryId { get; set; }              // ← add: the category this key belongs to
    public int Number { get; set; }                  // ← add: the number within it (BILL-3 → 3)
    public string TicketNumber { get; set; } = "";   // the retired key, e.g. "BILL-3"
    public DateTime CreatedAt { get; set; }          // when the ticket moved away from it
}