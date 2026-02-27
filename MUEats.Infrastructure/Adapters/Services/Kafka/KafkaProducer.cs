using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Adapters.Services.Kafka;

public class KafkaProducer : IProducer
{
    public Task ProduceAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}