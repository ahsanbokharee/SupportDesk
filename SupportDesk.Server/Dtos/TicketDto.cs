namespace SupportDesk.Server.Dtos;

public class TicketDto
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string Priority { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public string CategoryColor { get; set; } = "";
    public List<string> PreviousNumbers { get; set; } = new();
}