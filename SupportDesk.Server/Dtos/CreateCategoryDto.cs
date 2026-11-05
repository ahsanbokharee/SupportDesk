using System.ComponentModel.DataAnnotations;

namespace SupportDesk.Server.Dtos;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [RegularExpression("^[A-Z]{2,6}$", ErrorMessage = "Prefix must be 2 to 6 capital letters.")]
    public string Prefix { get; set; } = "";

    [Required]
    [AllowedValues("Low", "Medium", "High")]
    public string DefaultPriority { get; set; } = "Medium";

    [Required]
    [RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Colour must be a hex value like #1e88e5.")]
    public string Color { get; set; } = "#607d8b";
}