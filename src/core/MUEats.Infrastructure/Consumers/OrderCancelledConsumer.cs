using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Consumers;

public class OrderCancelledConsumer : BaseConsumer<OrderCancelledEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderCancelledConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(OrderCancelledEvent message, CancellationToken ct)
    {
        await using var scope =  _scopeFactory.CreateAsyncScope();
        
        var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();
        
        await inboxService.AddAsync(message, ct);
    }
}