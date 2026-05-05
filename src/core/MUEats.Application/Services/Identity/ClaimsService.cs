using System.Security.Claims;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;

namespace MUEats.Application.Services.Identity;

public class ClaimsService : IClaimsService
{
    private readonly IEnumerable<IClaimStrategy> _claimStrategies;

    public ClaimsService(IEnumerable<IClaimStrategy> claimStrategies)
    {
        _claimStrategies = claimStrategies;
    }

    public IEnumerable<Claim> GetClaims(User user)
    {
        var strategy = _claimStrategies.FirstOrDefault(x => x.UserRole == user.Role);

        if (strategy is null)
        {
            throw new ArgumentException("No such role");
        }

        return strategy.GetClaims(user);
    }
}