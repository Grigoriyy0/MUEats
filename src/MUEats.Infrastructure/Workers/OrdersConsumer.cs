using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MUEats.Core;
using MUEats.Infrastructure.Options;
using MUEats.Infrastructure.Persistence;

namespace MUEats.Infrastructure.Workers;

internal sealed class OrdersConsumer : BackgroundService 
{
    private readonly IConsumer<Null, string> _consumer;
    
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ILogger<OrdersConsumer> _logger;

    public OrdersConsumer(IOptions<KafkaOptions> options, 
        IServiceScopeFactory scopeFactory, 
        ILogger<OrdersConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            GroupId = "Orders"
        };

        if (!options.Value.ConsumerOptions.TryGetValue("Order", out var consumerOptions))
        {
            throw new Exception("Orders consumer options not found");
        }
        
        _consumer = new ConsumerBuilder<Null, string>(config).
            Build();
        
        _consumer.Subscribe(consumerOptions.Topic);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var messageResult = _consumer.Consume(ct);
            
                var messageType = Encoding.UTF8.GetString(messageResult.Message.Headers.GetLastBytes("message-type"));
            
                var message = messageResult.Message.Value;

                await ProcessMessageAsync(message, messageType, ct);
                
                _consumer.Commit(messageResult);
            }
            catch (Exception e)
            {
                _logger.LogError("Caught exception : {0}", e.Message);
            }
        }
    }

    private async Task ProcessMessageAsync(string message, string messageType, CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbCtx = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        
        var inboxMessage = new InboxMessage
        {
            Id = Guid.NewGuid(),
            Type = messageType,
            JsonPayload = message,
            CreatedAt = DateTime.UtcNow
        };

        await dbCtx.InboxMessages.AddAsync(inboxMessage, ct);
        await dbCtx.SaveChangesAsync(ct);
    }
}