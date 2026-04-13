using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Workers;

internal sealed class OrdersConsumer : BackgroundService 
{
    private readonly IConsumer<Null, string> _consumer;
    
    private readonly IServiceScopeFactory _scopeFactory;

    public OrdersConsumer(IOptions<KafkaOptions> options, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
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
            
                await using var scope = _scopeFactory.CreateAsyncScope();
            
                var eventDispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                await eventDispatcher.DispatchAsync(messageType, message, ct);
                
                _consumer.Commit(messageResult);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}