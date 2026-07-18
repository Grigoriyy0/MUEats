using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class RoleUpdatedConsumer : BaseConsumer<RoleUpdatedEvent>
{
    public RoleUpdatedConsumer(IOptions<KafkaOptions> options) : base(options)
    {
    }

    protected override Task ProcessMessageAsync(RoleUpdatedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}