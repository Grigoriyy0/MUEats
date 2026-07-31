using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Notifications.Core.Domain;
using MUEats.Notifications.Infrastructure.Options;
using MUEats.Notifications.Infrastructure.Persistence;
using SharedContracts.IntegrationEvents;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MUEats.Notifications.Infrastructure.Adapters.Kafka;

public class OrderAcceptedConsumer : BaseConsumer<OrderAcceptedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderAcceptedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbCtx = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();

        var exists = await dbCtx.Notifications
            .AnyAsync(x => x.CorrelationId == message.Id && x.Type == NotificationType.Email, ct);

        if (exists) return;

        var eventDataJson = JsonSerializer.Serialize(new 
        {
            message.OrderId,
            message.PickUpTime,
            message.TotalAmount
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