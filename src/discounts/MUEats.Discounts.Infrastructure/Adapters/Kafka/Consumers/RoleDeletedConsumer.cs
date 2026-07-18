using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class RoleDeletedConsumer : BaseConsumer<RoleDeletedEvent>
{
    public RoleDeletedConsumer(IOptions<KafkaOptions> options) : base(options)
    {
    }

    protected override Task ProcessMessageAsync(RoleDeletedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}