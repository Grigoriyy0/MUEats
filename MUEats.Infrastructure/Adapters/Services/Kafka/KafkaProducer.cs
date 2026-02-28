using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Options;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Services.Kafka;

public class KafkaProducer : IProducer
{
    private readonly IProducer<Null, string> _producer;

    private readonly TopicMapper _topicMapper;

    public KafkaProducer(IOptions<KafkaOptions> options, TopicMapper topicMapper)
    {
        _topicMapper = topicMapper;
        
        var kafkaOptions = options.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.BootstrapServers,
            AllowAutoCreateTopics = false
        };
        
        _producer = new ProducerBuilder<Null, string>(producerConfig)
            .Build();
    }
    
    public async Task ProduceAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        var json = JsonConvert.SerializeObject(message, Formatting.Indented);

        var topic = _topicMapper.TryGetTopic(message!.GetType());
        
        ArgumentNullException.ThrowIfNull(topic);
        
        var kafkaMessage = new Message<Null, string>
        {
            Value = json
        };

        await _producer.ProduceAsync(topic, kafkaMessage, ct);
    }
}