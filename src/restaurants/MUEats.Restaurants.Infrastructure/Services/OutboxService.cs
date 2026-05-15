using MUEats.Restaurants.Application.Helpers;
using MUEats.Restaurants.Application.IntegrationEvents;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;
using MUEats.Restaurants.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace MUEats.Restaurants.Infrastructure.Services;

public class OutboxService : IOutboxService
{
    private readonly RestaurantsDbContext _context;

    public OutboxService(RestaurantsDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(IntegrationEvent @event, CancellationToken ct)
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
        
        return _context.OutboxMessages.AddAsync(outboxMessage, ct)
            .AsTask();
    }
}