using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class RestaurantDeletedConsumer : BaseConsumer<RestaurantDeletedEvent>
{
    public RestaurantDeletedConsumer(IOptions<KafkaOptions> options) : base(options)
    {
    }

    protected override Task ProcessMessageAsync(RestaurantDeletedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}