using MUEats.Restaurants.Application.IntegrationEvents;
using SharedContracts.IntegrationEvents;

namespace MUEats.Restaurants.Infrastructure.Services.Interfaces;

public interface IInboxService
{
    Task AddAsync(IntegrationEvent message, CancellationToken ct);
}