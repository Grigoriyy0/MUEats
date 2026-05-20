using MUEats.Core.Domain.User.Entities;

namespace MUEats.Core.Domain.User;

public class User
{
    public Guid Id { get; init; }

    public string Username { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;
    
    public Role Role { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];

    public List<UserAttribute> UserAttributes { get; set; } = [];
}