using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MUEats.Restaurants.Application.Helpers;
using MUEats.Restaurants.Infrastructure.Options;
using Newtonsoft.Json;

namespace MUEats.Restaurants.Infrastructure.Adapters.Kafka.Consumers;

public abstract class BaseConsumer<T> : BackgroundService
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly KafkaOptions _kafkaOptions;

    public BaseConsumer(IOptions<KafkaOptions> options)
    {
        _kafkaOptions = options.Value;

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            AllowAutoCreateTopics = false,
            GroupId = typeof(T).Name,
        };
        
        _consumer = new ConsumerBuilder<Null, string>(consumerConfig)
            .Build();

        if (!_kafkaOptions.ConsumerOptions.TryGetValue(typeof(T).Name, out var consumerOptions))
        {
            throw new ArgumentException("No such options");
        }
        
        _consumer.Subscribe(consumerOptions.Topic);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var messageResult = _consumer.Consume(ct);
            
            var message = JsonConvert.DeserializeObject<T>(messageResult.Message.Value, JsonSerializerHelper.Settings);

            if (message == null)
            {
                continue;
            }

            try
            {
                await ProcessMessageAsync(message, ct);
                _consumer.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
    
    protected abstract Task ProcessMessageAsync(T message, CancellationToken ct);
}