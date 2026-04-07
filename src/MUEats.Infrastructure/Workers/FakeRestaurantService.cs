using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MUEats.Application.Helpers;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Infrastructure.Options;
using MUEats.Infrastructure.Persistence;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Workers;

public class FakeRestaurantService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<Null, string> _consumer;

    public FakeRestaurantService(
        IServiceScopeFactory scopeFactory,
        IOptions<KafkaOptions> options)
    {
        _scopeFactory = scopeFactory;
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            Acks = Acks.Leader,
            GroupId = "Restaurants"
        };
        
        _consumer = new ConsumerBuilder<Null, string>(consumerConfig)
            .Build();
        
        if (!options.Value.ConsumerOptions.TryGetValue("Restaurants", out var consumerOptions))
        {
            throw new Exception("Orders consumer options not found");
        }
        
        _consumer.Subscribe(consumerOptions.Topic);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var messageResult = _consumer.Consume(ct);
            
            var @event = JsonConvert.DeserializeObject<OrderCreatedEvent>(messageResult.Message.Value);

            if (@event == null)
            {
                continue;
            }
            
            await HandleMessageAsync(@event, ct);
            
            _consumer.Commit(messageResult);
        }
    }

    private async Task HandleMessageAsync(OrderCreatedEvent message, CancellationToken ct)
    {
        await Task.Delay(5000, ct);
        
        await using var scope = _scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();
        
        var @event = new OrderAcceptedEvent
        {
            OrderId = message.OrderId,
        };
        
        var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            JsonPayload = json,
            CreatedAt = DateTime.UtcNow,
            Type = @event.GetType().Name
        };
        
        await dbContext.OutboxMessages.AddAsync(outboxMessage, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}