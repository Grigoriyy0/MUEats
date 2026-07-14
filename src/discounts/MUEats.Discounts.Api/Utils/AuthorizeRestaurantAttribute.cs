using Microsoft.AspNetCore.Mvc;
using MUEats.Discounts.Api.Filters;

namespace MUEats.Discounts.Api.Utils;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeRestaurantAttribute : TypeFilterAttribute
{
    public AuthorizeRestaurantAttribute() : base(typeof(RestaurantAuthorizationFilter))
    {
    }
}