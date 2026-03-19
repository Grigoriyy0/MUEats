using MUEats.Application.Dto.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Ports;

public interface IRestaurantsProvider
{
    Task<OrderStatus> CheckStatusAsync(Guid orderId, CancellationToken ct);

    Task<bool> SubmitOrderAsync(OrderDto order, CancellationToken ct);
}