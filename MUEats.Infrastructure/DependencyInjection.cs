using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Adapters.Repositories;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Options;
using MUEats.Infrastructure.Persistence;
using MUEats.Infrastructure.Workers;

namespace MUEats.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MueDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IRestaurantsRepository, RestaurantsRepository>();
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IShoppingCartsRepository, ShoppingCartsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IFoodItemsRepository, FoodItemsRepository>();
        services.AddSingleton<IHashProvider, HashProvider>();
        services.AddScoped<ITokenProducer, TokenProducer>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();

        services.AddSingleton<TopicMapper>();
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        
        services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));
        
        services.AddHostedService<OutboxProcessingWorker>();
    }
}