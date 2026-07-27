using System.ComponentModel.DataAnnotations;

namespace MUEats.Identity.Application.Dtos;

public sealed record SignupDto
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    [Compare("Password")]
    public string PasswordConfirmation { get; set; } = null!;
}