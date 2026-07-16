using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Application.Handlers;

public class RestaurantDeletedHandler : IIntegrationEventHandler<RestaurantDeletedEvent>
{
    public Task HandleAsync(RestaurantDeletedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}