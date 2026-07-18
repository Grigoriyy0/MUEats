namespace MUEats.Discounts.Api.Utils;

public class CurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid? GetRestaurantId()
    {
        var idClaim = _accessor.HttpContext?.User.FindFirst("restaurant_id")?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var restaurantId))
        {
            return null;
        }

        return restaurantId;
    }

    public bool IsInRole(string roleName)
    {
        return _accessor.HttpContext?.User.IsInRole(roleName) ?? false;
    }
}