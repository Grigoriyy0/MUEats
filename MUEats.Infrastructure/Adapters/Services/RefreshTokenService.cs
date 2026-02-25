using Microsoft.Extensions.Options;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Options;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Services;

public class RefreshTokenService(
    MueDbContext context,
    IOptions<AuthOptions> options)
{
    private readonly AuthOptions _options = options.Value;
    
    public async Task SaveAsync(Guid userId, string token, CancellationToken ct)
    {
        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = token,
            UserId = userId,
            IsRevoked = false,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.Add(TimeSpan.FromDays(_options.RefreshTokenExpirationDays))
        };

        await context.RefreshTokens.AddAsync(newToken, ct);
        await context.SaveChangesAsync(ct);
    }
}