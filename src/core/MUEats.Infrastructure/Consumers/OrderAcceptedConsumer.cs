using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.IntegrationEvents;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Consumers;

public class OrderAcceptedConsumer : BaseConsumer<OrderAcceptedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderAcceptedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(OrderAcceptedEvent message, CancellationToken ct)
    {
        await using var scope =  _scopeFactory.CreateAsyncScope();
        
        var inboxService = scope.ServiceProvider.GetRequiredService<InboxService>();
        
        await inboxService.AddAsync(message, ct);
    }
}