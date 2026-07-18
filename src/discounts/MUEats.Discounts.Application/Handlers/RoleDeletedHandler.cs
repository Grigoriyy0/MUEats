using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Application.Handlers;

public class RoleDeletedHandler : IIntegrationEventHandler<RoleDeletedEvent>
{
    public Task HandleAsync(RoleDeletedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}