using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Server.Dtos;

public class UpdateUserDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required, MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = "";

    [Required, AllowedValues("Customer", "Agent", "Admin")]
    public string Role { get; set; } = "";

    public bool IsActive { get; set; }
}