using System.Security.Claims;
using MUEats.Core.Domain.User;

namespace MUEats.Application.Interfaces;

public interface IClaimsService
{
    IEnumerable<Claim> GetClaims(User user);
}