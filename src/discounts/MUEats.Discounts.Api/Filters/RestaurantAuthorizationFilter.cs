using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MUEats.Discounts.Api.Utils;
using MUEats.Discounts.Application.Dtos;

namespace MUEats.Discounts.Api.Filters;

public class RestaurantAuthorizationFilter : IAsyncActionFilter
{
    private readonly CurrentUserContext _context;

    public RestaurantAuthorizationFilter(CurrentUserContext context)
    {
        _context = context;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (_context.IsInRole("Admin"))
        {
            await next();
            return;
        }
        
        if (_context.IsInRole("RestaurantOwner"))
        {
            Guid? requestRestaurantId = null;
            
            var restaurantId = _context.GetRestaurantId();

            foreach (var argumentsValue in context.ActionArguments.Values)
            {
                if (argumentsValue is IRestaurantRelatedRequest request)
                {
                    requestRestaurantId = request.RestaurantId;
                    break;
                }
            }

            if (requestRestaurantId == null && context.RouteData.Values.TryGetValue("restaurantId", out var routeValue))
            {
                if (Guid.TryParse(routeValue?.ToString(), out var parsedId))
                {
                    requestRestaurantId = parsedId;
                }
            }

            if (restaurantId == null || requestRestaurantId != restaurantId)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        await next();
    }
}