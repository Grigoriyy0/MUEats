using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Handlers;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Interfaces;
using MUEats.Application.Services;

namespace MUEats.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ShoppingCartsService>();
        
        services.AddScoped<IIdentityManager, IdentityManager>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRolesService, RolesService>();
        
        services.AddScoped<IIntegrationEventHandler<OrderAcceptedEvent>, OrderAcceptedHandler>();
        services.AddScoped<IIntegrationEventHandler<OrderPreparedEvent>, OrderPreparedHandler>();
        services.AddScoped<IIntegrationEventHandler<OrderRejectedEvent>, OrderRejectedHandler>();
        services.AddScoped<IIntegrationEventHandler<OrderCancelledEvent>, OrderCancelledHandler>();
    }
}