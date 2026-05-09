namespace MUEats.Application.Handlers;

public interface IIntegrationEventHandler<in TMessage>
{
    public Task HandleAsync(TMessage message, CancellationToken ct);
}