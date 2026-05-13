using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Application.Helpers;
using MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using MUEats.Restaurants.Infrastructure.Persistence.Inbox;
using MUEats.Restaurants.Infrastructure.Services.Interfaces;
using Newtonsoft.Json;

namespace MUEats.Restaurants.Infrastructure.Services;

public class InboxService : IInboxService
{
    private readonly RestaurantsDbContext _context;

    public InboxService(RestaurantsDbContext context)
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