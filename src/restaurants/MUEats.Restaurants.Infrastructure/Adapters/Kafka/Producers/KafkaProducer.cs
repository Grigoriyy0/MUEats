using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MUEats.Restaurants.Application.Helpers;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure.Options;
using MUEats.Restaurants.Infrastructure.Services;
using Newtonsoft.Json;

namespace MUEats.Restaurants.Infrastructure.Adapters.Kafka.Producers;

public class KafkaProducer : IMessageBus
{
    private readonly IProducer<Null, string> _producer;
    private readonly TopicMapper _topicMapper;
    
    public KafkaProducer(IOptions<KafkaOptions> options, TopicMapper topicMapper)
    {
        _topicMapper = topicMapper;
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = false,
            Acks = Acks.Leader
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig)
            .Build();
    }
    
    public async Task ProduceAsync<T>(T message, CancellationToken ct)
    {
        var json = JsonConvert.SerializeObject(message, JsonSerializerHelper.Settings);

        var topic = _topicMapper.TryGetTopic(message!.GetType());

        ArgumentNullException.ThrowIfNull(topic);
        
        var kafkaMessage = new Message<Null, string>
        {
            Value = json,
        };

        await _producer.ProduceAsync(topic, kafkaMessage, ct);
    }
}