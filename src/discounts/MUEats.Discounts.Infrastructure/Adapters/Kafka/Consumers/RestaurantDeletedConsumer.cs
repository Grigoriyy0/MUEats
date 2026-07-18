using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class RestaurantDeletedConsumer : BaseConsumer<RestaurantDeletedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public RestaurantDeletedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(RestaurantDeletedEvent message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbCtx = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();

        await dbCtx.Discounts.Where(x => x.RestaurantId == message.RestaurantId)
            .ExecuteDeleteAsync(ct);
    }
}