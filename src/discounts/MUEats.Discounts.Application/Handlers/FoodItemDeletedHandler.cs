using SharedContracts.IntegrationEvents;

namespace MUEats.Discounts.Application.Handlers;

public class FoodItemDeletedHandler : IIntegrationEventHandler<FoodItemDeletedEvent>
{
    public Task HandleAsync(FoodItemDeletedEvent message, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}