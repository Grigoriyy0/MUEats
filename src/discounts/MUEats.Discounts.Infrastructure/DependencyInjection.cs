using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;

namespace MUEats.Discounts.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DiscountsDbContext>(opt => 
            opt.UseNpgsql(configuration.GetConnectionString("Postgres")));
    }
}