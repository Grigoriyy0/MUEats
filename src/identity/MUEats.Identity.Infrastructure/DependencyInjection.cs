using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Infrastructure.Adapters;
using MUEats.Identity.Infrastructure.Persistence.DbContexts;

namespace MUEats.Identity.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenProducer, JwtTokenProducer>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IHashProvider, HashProvider>();
        
        services.AddDbContext<IdentityDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Postgres"))
            .UseSnakeCaseNamingConvention());
    }
}