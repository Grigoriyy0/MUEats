using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Ports;

public interface IRefreshTokenService
{
    Task SaveAsync(Guid userId, string token, CancellationToken ct);

    Task<RefreshToken?> GetAsync(string token, CancellationToken ct);
}