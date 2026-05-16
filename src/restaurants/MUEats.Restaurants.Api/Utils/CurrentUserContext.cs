namespace MUEats.Restaurants.Api.Utils;

public class CurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public Guid GetRestaurantId()
    {
        var idClaim = _accessor.HttpContext?.User.FindFirst("restaurant_id")?.Value;

        if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }
}