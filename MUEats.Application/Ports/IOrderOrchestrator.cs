using MUEats.Core.Domain.Order;

namespace MUEats.Application.Ports;

public interface IOrderOrchestrator
{
    Task<Guid> StartAsync(Order order, CancellationToken ct);
}