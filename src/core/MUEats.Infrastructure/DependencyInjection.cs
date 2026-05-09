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
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IShoppingCartsRepository, ShoppingCartsRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITokenProducer, TokenProducer>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IOrderSagaStatesRepository, OrderSagaStatesRepository>();
        services.AddScoped<EventDispatcher>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<IOrdersQueries, OrdersQueries>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<InboxService>();
        services.AddScoped<IOrdersService, OrdersService>();
        
        services.AddSingleton<IHashProvider, HashProvider>();
        services.AddSingleton<IPasswordValidator, PasswordValidator>();
        services.AddSingleton<TopicMapper>();
        services.AddSingleton<IProducer, KafkaProducer>();
        services.AddSingleton<EventsRegistry>();
        
        services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));
        services.Configure<PasswordValidatorOptions>(configuration.GetSection(nameof(PasswordValidatorOptions)));
        services.Configure<KafkaOptions>(configuration.GetSection(nameof(KafkaOptions)));
        services.Configure<AdminOptions>(configuration.GetSection(nameof(AdminOptions)));
        
        services.AddHostedService<FakeRestaurantService>();
        services.AddHostedService<OrdersConsumer>();
        
        services.AddHostedService<OutboxProcessingWorker>();
        services.AddHostedService<InboxProcessingWorker>();
        services.AddHostedService<OrderAcceptedConsumer>();
        
        services.AddSingleton<FakeKitchenWorker>();
        services.AddHostedService(sp => sp.GetRequiredService<FakeKitchenWorker>());
    }
}