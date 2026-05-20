namespace MUEats.Restaurants.Application.Ports;

public interface IMessageBus
{
    Task ProduceAsync<T>(T message, CancellationToken ct);
}