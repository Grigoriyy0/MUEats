using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class FoodItemDeletedConsumer : BaseConsumer<FoodItemDeletedEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public FoodItemDeletedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory serviceScopeFactory) : base(options)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ProcessMessageAsync(FoodItemDeletedEvent message, CancellationToken ct)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var dbCtx = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();

        await dbCtx.Discounts.Where(x => x.FoodItemId == message.FoodItemId)
            .ExecuteDeleteAsync(ct);
    }
}