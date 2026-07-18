using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Discounts.Infrastructure.Options;
using MUEats.Discounts.Infrastructure.Persistence.Contexts;
using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Infrastructure.Adapters.Kafka.Consumers;

public class RoleDeletedConsumer : BaseConsumer<RoleDeletedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public RoleDeletedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(RoleDeletedEvent message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbCtx = scope.ServiceProvider.GetRequiredService<DiscountsDbContext>();

        await dbCtx.IdentityRoles.Where(x => x.RoleName == message.RoleName || x.RoleId == message.RoleId)
            .ExecuteDeleteAsync(ct);
    }
}