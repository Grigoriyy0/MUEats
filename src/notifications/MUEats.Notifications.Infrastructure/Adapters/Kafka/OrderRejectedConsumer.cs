using Microsoft.Extensions.Options;
using MUEats.Notifications.Infrastructure.Options;
using SharedContracts.IntegrationEvents;

namespace MUEats.Notifications.Infrastructure.Adapters.Kafka;

public class OrderRejectedConsumer : BaseConsumer<OrderRejectedEvent>
{
    public OrderRejectedConsumer(IOptions<KafkaOptions> options) : base(options)
    {
    }
    
    protected override Task ProcessMessageAsync(OrderRejectedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}