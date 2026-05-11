using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Consumers;

public class OrderPreparedConsumer : BaseConsumer<OrderPreparedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public OrderPreparedConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory) : base(options)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ProcessMessageAsync(OrderPreparedEvent message, CancellationToken ct)
    {
        await using var scope =  _scopeFactory.CreateAsyncScope();
        
        var inboxService = scope.ServiceProvider.GetRequiredService<IInboxService>();
        
        await inboxService.AddAsync(message, ct);
    }
}