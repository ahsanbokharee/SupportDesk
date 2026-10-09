

using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Server.Dtos;

public class UpdateTicketDto
{
    [Required]
    [MaxLength(200)]
    public string Subject {get; set;} = "";
    [Required]
    [MaxLength(200)]
    public string Description {get; set;} = "";
    [Required]
    [AllowedValues ("Low", "Medium", "High")]
    public string Priority {get; set;} = "";    
    [Required]
    [AllowedValues ("Open", "Closed", "In Progress", "Resolved")]
    public string Status {get; set;} = "";
}