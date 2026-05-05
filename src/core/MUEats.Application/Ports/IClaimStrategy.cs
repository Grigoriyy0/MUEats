using System.Security.Claims;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Ports;

public interface IClaimStrategy
{
    Role UserRole { get; }

    IEnumerable<Claim> GetClaims(User user);
}