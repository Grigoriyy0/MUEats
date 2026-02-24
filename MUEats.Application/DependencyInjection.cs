using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Services;

namespace MUEats.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<RestaurantsService>();
        services.AddScoped<ShoppingCartsService>();
    }
}