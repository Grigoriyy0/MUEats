using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Core.Projections.Order;
using MUEats.Restaurants.Infrastructure.Options;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using MUEats.Restaurants.Infrastructure.Services.Interfaces;

namespace MUEats.Restaurants.Infrastructure.Adapters.Kafka.Consumers;

public class OrderCreatedConsumer : BaseConsumer<OrderCreatedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderCreatedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var ctx = scope.ServiceProvider.GetRequiredService<RestaurantsDbContext>();

        var exists = await ctx.OrderSnapshots.AnyAsync(x => x.OrderId == message.OrderId, ct);

        if (exists)
        {
            return;
        }

        var orderSnapshot = new OrderSnapshot
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            Status = OrderStatus.Created
        };

        await ctx.OrderSnapshots.AddAsync(orderSnapshot, ct);
    }
}