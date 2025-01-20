using System.ComponentModel.DataAnnotations;
namespace shop.Models;
public class RegisterModel
{
    [Required]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    
    public string Role { get; set; } = "user"; // Sprawdzic czy rejestrujace sie user nie moze wymusic sobie admina
}