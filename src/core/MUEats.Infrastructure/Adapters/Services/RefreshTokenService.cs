using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User.Entities;
using MUEats.Infrastructure.Options;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Adapters.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthOptions _options;
    private readonly MueDbContext _context;

    public RefreshTokenService(MueDbContext context, IOptions<AuthOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public Task SaveAsync(Guid userId, string token, CancellationToken ct)
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

        return _context.RefreshTokens.AddAsync(newToken, ct).AsTask();
    }

    public Task<RefreshToken?> GetAsync(string token, CancellationToken ct)
    {
        return _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
    }

    public Task RevokeAllForUserAsync(Guid userId, CancellationToken ct)
    {
        return _context.RefreshTokens.Where(x => x.UserId == userId && !x.IsRevoked)
            .ExecuteUpdateAsync(y => y.SetProperty(t => t.IsRevoked, true), ct);
        
    }
}