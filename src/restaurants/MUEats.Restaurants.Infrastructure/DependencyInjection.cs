using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.Adapters;
using MUEats.Restaurants.Infrastructure.Adapters.Kafka.Consumers;
using MUEats.Restaurants.Infrastructure.Adapters.Kafka.Producers;
using MUEats.Restaurants.Infrastructure.Options;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using MUEats.Restaurants.Infrastructure.Services;
using MUEats.Restaurants.Infrastructure.Services.Interfaces;
using MUEats.Restaurants.Infrastructure.Workers;
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
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOrderSnapshotsRepository, OrderSnapshotsRepository>();
        
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        services.AddSingleton<IPresenceService, PresenceService>();
        services.AddSingleton<TopicMapper>();
        services.AddSingleton<IMessageBus, KafkaProducer>();
        
        services.AddHostedService<OrderCreatedConsumer>();
        services.AddHostedService<OutboxProcessingWorker>();

        services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));
    }
}