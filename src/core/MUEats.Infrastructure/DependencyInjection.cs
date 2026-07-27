using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Adapters.Kafka;
using MUEats.Infrastructure.Adapters.Repositories;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Consumers;
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

        services.AddHttpContextAccessor();
        
        services.AddScoped<IOrdersRepository, OrdersRepository>();
        services.AddScoped<IShoppingCartsRepository, ShoppingCartsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderSagasRepository, OrderSagasRepository>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddScoped<IOrdersQueries, OrdersQueries>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IInboxService, InboxService>();
        services.AddScoped<IOrdersService, OrdersService>();
        
        services.AddSingleton<IHashProvider, HashProvider>();
        services.AddSingleton<TopicMapper>();
        services.AddSingleton<IProducer, KafkaProducer>();
        
        services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));
        
        services.AddHostedService<OutboxProcessingWorker>();
        services.AddHostedService<InboxProcessingWorker>();
        services.AddHostedService<OrderCancellationJob>();
        
        services.AddHostedService<OrderAcceptedConsumer>();
        services.AddHostedService<OrderPreparedConsumer>();
        services.AddHostedService<OrderRejectedConsumer>();
        services.AddHostedService<OrderCancelledConsumer>();
    }
}