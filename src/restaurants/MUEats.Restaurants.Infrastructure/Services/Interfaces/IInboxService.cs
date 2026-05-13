using MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;

namespace MUEats.Restaurants.Infrastructure.Services.Interfaces;

public interface IInboxService
{
    Task AddAsync(IntegrationEvent message, CancellationToken ct);
}