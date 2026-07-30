using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Notifications.Infrastructure.Persistence;

namespace MUEats.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationsDbContext>(opt => 
            opt.UseNpgsql(configuration.GetConnectionString("Postgres")));
    }
}