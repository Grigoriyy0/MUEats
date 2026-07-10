namespace MUEats.Application.Dto.User;

public sealed record AuthDto
{
    public string Email { get; set; }
    
    public string Password { get; set; }
}