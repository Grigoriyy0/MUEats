using MUEats.Application.Dto.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Ports;

public interface IOrdersQueries
{
    Task<OrderStatus> GetStatusAsync(Guid id, CancellationToken ct);

    Task<OrderDto?> GetDtoByIdAsync(Guid orderId, CancellationToken ct);
}