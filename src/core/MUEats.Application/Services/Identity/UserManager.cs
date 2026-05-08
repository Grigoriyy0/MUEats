using System.Security.Claims;

namespace MUEats.Application.Services.Identity;

public static class UserManager
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst("id");

        if (userId == null)
        {
            throw new ArgumentException();
        }
        
        return Guid.Parse(userId.Value);
    }
}