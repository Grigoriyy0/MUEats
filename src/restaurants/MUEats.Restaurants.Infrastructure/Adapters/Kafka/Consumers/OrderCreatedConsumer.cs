using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;
using MUEats.Restaurants.Infrastructure.Options;
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

        var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();

        await inboxService.AddAsync(message, ct);
    }
}