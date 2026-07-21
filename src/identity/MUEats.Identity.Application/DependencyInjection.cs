using Microsoft.Extensions.DependencyInjection;
using MUEats.Identity.Application.Interfaces;
using MUEats.Identity.Application.Services;

namespace MUEats.Identity.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityManager, IdentityManager>();
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IRolesService, RolesService>();
    }
}