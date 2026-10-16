using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Server.Dtos;

public class CreateUserDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = "";

    [Required, MaxLength(100)]
    public string LastName { get; set; } = "";

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    public string Password { get; set; } = "";

    [Required, AllowedValues("Customer", "Agent", "Admin")]
    public string Role { get; set; } = "";
}