using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RestaurantsDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("Postgres"))
                .UseSnakeCaseNamingConvention());
    }
}