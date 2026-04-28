using MUEats.Core.Domain.User.Entities;
using MUEats.Core.Domain.User.Utils;

namespace MUEats.Core.Domain.User;

public class User
{
    public Guid Id { get; init; }

    public string Username { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? DefaultAddress { get; set; }
    
    public Role Role { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
}