using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Infrastructure.Adapters.Services;
using MUEats.Infrastructure.Options;
using Newtonsoft.Json;

namespace MUEats.Infrastructure.Adapters.Kafka;

public class KafkaProducer : IProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly TopicMapper _topicMapper;

    public KafkaProducer(IOptions<KafkaOptions>  options, TopicMapper topicMapper)
    {
        _topicMapper = topicMapper;
        
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            AllowAutoCreateTopics = false,
            Acks = Acks.Leader,
        };
        
        _producer = new ProducerBuilder<Null, string>(config)
            .Build();
    }

    public async Task ProduceAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        var json = JsonConvert.SerializeObject(message, JsonSerializerHelper.Settings);
        
        var topic = _topicMapper.TryGetTopic(message!.GetType());
        
        ArgumentNullException.ThrowIfNull(topic);

        var kafkaMessage = new Message<Null, string>()
        {
            Value = json,
        };
        
        await _producer.ProduceAsync(topic, kafkaMessage, ct);
    }
}