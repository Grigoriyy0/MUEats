using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Notifications.Core.Domain;
using MUEats.Notifications.Infrastructure.Options;
using MUEats.Notifications.Infrastructure.Persistence;
using SharedContracts.IntegrationEvents;

namespace MUEats.Notifications.Infrastructure.Adapters.Kafka;

public class OrderRejectedConsumer : BaseConsumer<OrderRejectedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderRejectedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ProcessMessageAsync(OrderRejectedEvent message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        
        var dbCtx = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var exists = await dbCtx.Notifications
            .AnyAsync(x => x.CorrelationId == message.Id && x.Type == NotificationType.Email, ct);

        if (exists) return;

        var eventDataJson = JsonSerializer.Serialize(new 
        {
            message.OrderId,
            message.Reason
        });

        var result = Notification.Create(
            userId: message.UserId,
            correlationId: message.Id,
            eventData: eventDataJson,
            recipientInfo: message.ContactInfo,
            type: NotificationType.Email
        );

        if (result.IsFailure)
        {
            return;
        }

        dbCtx.Notifications.Add(result.Value);
        await dbCtx.SaveChangesAsync(ct);
    }
}