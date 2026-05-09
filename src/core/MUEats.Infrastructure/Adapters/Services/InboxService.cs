using Microsoft.EntityFrameworkCore;
using MUEats.Application.Helpers;
using MUEats.Application.IntegrationEvents;
using MUEats.Core;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services;

public class InboxService
{
    private readonly MueDbContext _context;

    public InboxService(MueDbContext context)
    {
        _context = context;
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
}