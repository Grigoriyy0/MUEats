using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.IntegrationEvents;
using MUEats.Core.Domain.Order.ValueObjects;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Workers;

public class OrderCancellationJob : BackgroundService
{
    private const int BatchSize = 50;
    private readonly IServiceScopeFactory _scopeFactory;
    private const double MinutesLength = 10;

    public OrderCancellationJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return WorkingLoop(ct);
    }

    private async Task WorkingLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();

            var dbCtx = scope.ServiceProvider.GetRequiredService<MueDbContext>();
            var outboxService = scope.ServiceProvider.GetRequiredService<OutboxService>();

            var expirationDateTime = DateTime.UtcNow.AddMinutes(-1 * MinutesLength);
            
            var ordersIds = await dbCtx.Orders.AsNoTracking()
                .Where(x => x.Status == OrderStatus.Created && x.CreatedAt < expirationDateTime)
                .Select(y => y.Id)
                .Take(BatchSize)
                .ToListAsync(ct);

            if (ordersIds.Count == 0)
            {
                await Task.Delay(300, ct);
            }
            
            foreach (var id in ordersIds)
            {
                var @event = new OrderRejectedEvent
                {
                    OrderId = id,
                    Reason = "Timeout"
                };

                await outboxService.CreateAsync(@event, ct);
            }

            await dbCtx.SaveChangesAsync(ct);
        }
    }

}