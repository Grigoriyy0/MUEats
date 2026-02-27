using MUEats.Application.Dto.Order;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;
using Newtonsoft.Json;

namespace MUEats.Application.Services;

public class OrdersService(
    IShoppingCartsRepository shoppingCartsRepository,
    IOrdersRepository ordersRepository,
    IUnitOfWork uow,
    IOutboxRepository outboxRepository
    )
{
    public async Task<Guid> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);
        
            var cart = await shoppingCartsRepository.GetCartDtoAsync(dto.UserId, ct);

            if (cart is null)
            {
                throw new ArgumentException("Add items to create an order.");
            }

            var totalPrice = cart.Items.Sum(x => x.Price);
        
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Price = totalPrice,
                Address = dto.Address,
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = i.FoodItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                Status = OrderStatus.Created
            };

            await ordersRepository.AddAsync(order, ct);

            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id
            };

            var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                JsonPayload = json,
                Type = @event.GetType().Name,
            };

            await outboxRepository.AddAsync(outboxMessage, ct);
            
            await uow.SaveChangesAsync(ct);
            await uow.CommitTransactionAsync(ct);
        
            return order.Id;
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<string?> GetStatusAsync(Guid orderId, CancellationToken ct)
    {
        var status = await ordersRepository.GetStatusAsync(orderId, ct);

        if (status is null)
        {
            throw new ArgumentException($"Order with {orderId} not found");
        }
        
        return status.ToString();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var dto = await ordersRepository.GetDtoByIdAsync(orderId, ct);

        if (dto is null)
        {
            throw new ArgumentException($"Order with {orderId} is not found");
        }

        return dto;
    }
}