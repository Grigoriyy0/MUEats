using Microsoft.EntityFrameworkCore;
using MUEats.Application.Helpers;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.Metrics;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class InboxService : IInboxService
{
    private readonly MueDbContext _context;
    private readonly IEventDispatcher _dispatcher;
    private const int RetryMaxCount = 3;
    
    public InboxService(MueDbContext context, IEventDispatcher dispatcher)
    {
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task AddAsync(IntegrationEvent message, CancellationToken ct)
    {
        if (await _context.InboxMessages.AnyAsync(x => x.Id == message.Id, ct))
        {
            return;            
        }

        var json = JsonConvert.SerializeObject(message, JsonSerializerHelper.Settings);
        
        var inboxMessage = new InboxMessage
        {
            Id = message.Id,
            CreatedAt = DateTime.UtcNow,
            Type = message.GetType().Name,
            JsonPayload = json,
            Status = InboxStatus.Pending
        };
        
        await _context.AddAsync(inboxMessage, ct);
        await _context.SaveChangesAsync(ct);
    }


    public async Task ProcessMessageAsync(InboxMessage message, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        await using var tx = await _context.Database.BeginTransactionAsync(ct);

        message.LockId = null;
        
        try
        {
            var @event = JsonConvert.DeserializeObject<IntegrationEvent>(message.JsonPayload, JsonSerializerHelper.Settings);

            if (@event is null)
            {
                return;
            }
            
            await _dispatcher.DispatchAsync(@event, ct);
            
            message.ProcessedAt = DateTime.UtcNow;
            message.Status = InboxStatus.Processed;
            
            await _context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            
            InboxMetrics.InboxProcessed.WithLabels("success").Inc();
            InboxMetrics.InboxLag.Observe((DateTime.UtcNow - message.CreatedAt).TotalSeconds);
        }
        catch (Exception e)
        {
            await tx.RollbackAsync(ct);
            
            _context.ChangeTracker.Clear();
            _context.Entry(message).State = EntityState.Modified;
            
            message.LastError = e.Message;
            message.RetryCount++;

            if (message.RetryCount >= RetryMaxCount)
            {
                message.Status = InboxStatus.Failed;
            }
            else
            {
                var delay = Math.Pow(2, message.RetryCount - 1);
                message.NextRetryAt = DateTime.UtcNow.AddMinutes(delay);
            }
            
            await  _context.SaveChangesAsync(ct);
            
            InboxMetrics.InboxProcessed.WithLabels("failure").Inc();
        }
    }
}