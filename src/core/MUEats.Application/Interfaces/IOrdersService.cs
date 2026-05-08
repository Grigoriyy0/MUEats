using MUEats.Application.Dto.Order;

namespace MUEats.Application.Interfaces;

public interface IOrdersService
{
    Task<Guid> CreateAsync(Guid userId, CreateOrderDto dto, CancellationToken ct);
}