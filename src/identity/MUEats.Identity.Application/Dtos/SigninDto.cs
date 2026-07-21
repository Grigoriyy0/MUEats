namespace MUEats.Identity.Application.Dtos;

public sealed record SigninDto
{
    public string Email { get; init; }
    
    public string Password { get; init; }
}