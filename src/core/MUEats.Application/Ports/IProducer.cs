namespace MUEats.Application.Ports;

public interface IProducer
{
    Task ProduceAsync<TMessage>(TMessage message, CancellationToken ct);
}