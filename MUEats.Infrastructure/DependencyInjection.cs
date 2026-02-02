using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MueDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("Postgres")));
    }
}