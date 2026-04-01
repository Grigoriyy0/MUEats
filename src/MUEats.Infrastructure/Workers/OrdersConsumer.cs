using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Workers;

public class OrdersConsumer
{
    private readonly IConsumer<Null, string> _consumer;

    public OrdersConsumer(IOptions<KafkaOptions> options)
    {
        var config = new ConsumerConfig();
        
        _consumer = new ConsumerBuilder<Null, string>(config).
            Build();
        
        _consumer.Subscribe("orders");
    }
}