using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Server.Dtos;

public class TicketQuery
{
    [MaxLength(100)]
    public string? Search { get; set; }

    [AllowedValues(null, "Open", "In Progress", "Resolved", "Closed")]
    public string? Status { get; set; }

    [AllowedValues(null, "Low", "Medium", "High")]
    public string? Priority { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}