using Microsoft.Extensions.DependencyInjection;
using MUEats.Restaurants.Application.Handlers;
using MUEats.Restaurants.Application.Handlers.Interfaces;
using MUEats.Restaurants.Application.Services;

namespace MUEats.Restaurants.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<MenusService>();
        services.AddScoped<RestaurantsService>();
        services.AddScoped<MenusQueries>();
        services.AddScoped<IOrderSnapshotCreatedHandler, OrderSnapshotCreatedHandler>();
        services.AddScoped<IOrderSnapshotCancelHandler, OrderSnapshotCancelHandler>();
        services.AddScoped<OrderSnapshotsService>();
    }
}