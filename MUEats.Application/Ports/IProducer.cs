namespace MUEats.Application.Ports;

public interface IProducer
{
    public Task ProduceAsync<TMessage>(TMessage message, CancellationToken ct);
}