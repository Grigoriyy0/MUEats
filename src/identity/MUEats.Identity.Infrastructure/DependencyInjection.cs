using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Infrastructure.Adapters;
using MUEats.Identity.Infrastructure.Options;
using MUEats.Identity.Infrastructure.Persistence;
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

        services.AddScoped<DatabaseSeeder>();
        
        services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));
        services.Configure<AdminOptions>(configuration.GetSection(nameof(AdminOptions)));
        
        services.AddDbContext<IdentityDbContext>(opt => opt.UseNpgsql(configuration.GetConnectionString("Postgres"))
            .UseSnakeCaseNamingConvention());
    }
}