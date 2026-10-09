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
}