using MUEats.Application.Helpers;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class OutboxService : IOutboxService
{
    private readonly MueDbContext _context;

    public OutboxService(MueDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(IntegrationEvent @event, CancellationToken ct)
    {
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            JsonPayload = json,
            CreatedAt = DateTime.UtcNow,
            Type = @event.GetType().Name,
            Status = OutboxStatus.Pending,
        };
        
        await _context.OutboxMessages.AddAsync(outboxMessage, ct);
    }
}