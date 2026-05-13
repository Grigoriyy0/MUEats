using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.Adapters;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using StackExchange.Redis;

namespace MUEats.Restaurants.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RestaurantsDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("Postgres"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        services.AddScoped<IMenusRepository, MenusRepository>();
        services.AddScoped<IPresenceService, PresenceService>();
        
        
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddSingleton<IPresenceService, PresenceService>();
    }
}