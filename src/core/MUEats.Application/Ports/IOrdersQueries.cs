using MUEats.Application.Dto.Order;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Ports;

public interface IOrdersQueries
{
    Task<OrderStatus> GetStatusAsync(Guid id, CancellationToken ct);

    Task<OrderDto?> GetDtoByIdAsync(Guid orderId, CancellationToken ct);

    Task<List<OrderDto>> GetHistoryAsync(Guid userId, GetOrdersHistoryQuery query, CancellationToken ct);
}