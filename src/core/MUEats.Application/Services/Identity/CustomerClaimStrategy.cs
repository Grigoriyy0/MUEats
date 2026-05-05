using System.Security.Claims;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services.Identity;

public class CustomerClaimStrategy : IClaimStrategy
{
    public Role UserRole => Role.Customer;

    public IEnumerable<Claim> GetClaims(User user)
    {
        yield return new Claim("id", user.Id.ToString());
        yield return new Claim("role", user.Role.ToString());
        yield return new Claim("email", user.Email);
    }
}