using Microsoft.Extensions.Options;
using MUEats.Notifications.Infrastructure.Options;
using SharedContracts.IntegrationEvents;

namespace MUEats.Notifications.Infrastructure.Adapters.Kafka;

public class OrderAcceptedConsumer : BaseConsumer<OrderAcceptedEvent>
{
    public OrderAcceptedConsumer(IOptions<KafkaOptions> options) : base(options)
    {
    }

    protected override Task ProcessMessageAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}