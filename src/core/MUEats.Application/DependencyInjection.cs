using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Application.Services;
using MUEats.Application.Services.Identity;

namespace MUEats.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ShoppingCartsService>();
        services.AddScoped<OrdersService>();
        
        services.AddScoped<IClaimStrategy, CustomerClaimStrategy>();
        services.AddScoped<IClaimStrategy, AdminClaimStrategy>();
        services.AddScoped<IClaimStrategy, RestaurantManagerStrategy>();

        services.AddScoped<IClaimsService, ClaimsService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

    }
}