using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Workers;

internal sealed class InboxProcessingWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public InboxProcessingWorker(IServiceScopeFactory scopeFactory)
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
            ct.ThrowIfCancellationRequested();
            
            await using var scope = _scopeFactory.CreateAsyncScope();

            var dbCtx = scope.ServiceProvider.GetRequiredService<MueDbContext>();

            var messages = await dbCtx.InboxMessages
                .Where(im => im.ProcessedAt == null)
                .OrderBy(im => im.CreatedAt)
                .Take(50)
                .ToListAsync(ct);

            if (messages.Count == 0)
            {
                await Task.Delay(1000, ct);
                continue;
            }
            
            foreach (var message in messages)
            {
                await ProcessMessageAsync(message, ct);
            }
        }
    }

    private async Task ProcessMessageAsync(InboxMessage message, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbCtx = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

        try
        {
            await eventDispatcher.DispatchAsync(message.Type, message.JsonPayload, ct);

            message.ProcessedAt = DateTime.UtcNow;
        }
        catch (Exception e)
        {
            message.LastError = e.Message;
        }
        
        await dbCtx.SaveChangesAsync(ct);
    }
}