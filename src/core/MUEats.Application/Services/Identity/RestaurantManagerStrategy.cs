using System.Security.Claims;
using MUEats.Application.Ports;
using MUEats.Core.Domain.User;
using MUEats.Core.Domain.User.Entities;

namespace MUEats.Application.Services.Identity;

public class RestaurantManagerStrategy : IClaimStrategy
{
    public Role UserRole => Role.RestaurantManager;
    
    public IEnumerable<Claim> GetClaims(User user)
    {
        var attribute = user.UserAttributes.FirstOrDefault(x => x.Key == "restaurant_id");

        if (attribute is null)
        {
            throw new ArgumentException();
        }
        
        yield return new Claim("id", user.Id.ToString());
        yield return new Claim("role", user.Role.ToString());
        yield return new Claim("email", user.Email);
        yield return new Claim("restaurant_id", attribute.Value);
    }
}