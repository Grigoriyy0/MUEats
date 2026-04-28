using MUEats.Application.Dto.Order;
using MUEats.Application.Helpers;
using MUEats.Application.Ports;
using MUEats.Core;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Core.Domain.Order.ValueObjects;
using Newtonsoft.Json;

namespace MUEats.Application.Services;

public class OrdersService(
    IShoppingCartsRepository shoppingCartsRepository,
    IOrdersRepository ordersRepository,
    IUnitOfWork uow,
    IOutboxRepository outboxRepository,
    IOrderSagaStatesRepository sagaStatesRepository
    )
{
    public async Task<Guid> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);
        
            var cart = await shoppingCartsRepository.GetCartDtoAsync(dto.UserId, ct);

            if (cart is null || cart.Items.Count == 0)
            {
                throw new ArgumentException("Add items to create an order.");
            }

            var totalPrice = cart.Items.Sum(x => x.Price);
        
            var order = new Order
            {
                Id = Guid.NewGuid(),
                Address = dto.Address,
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = i.FoodItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                TotalPrice = totalPrice,
                OrderStatus = OrderStatus.Created,
                OrderDate = DateTime.UtcNow,
                UserId = dto.UserId,
                RestaurantId = cart.RestaurantId,
            };

            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id,
            };
            
            var json = JsonConvert.SerializeObject(@event, JsonSerializerHelper.Settings);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = @event.GetType().Name,
                JsonPayload = json,
                CreatedAt = DateTime.UtcNow,
            };

            var sagaState = new OrderSagaState
            {
                CorrelationId = order.Id,
                State = SagaStatus.Created
            };
            
            await ordersRepository.AddAsync(order, ct);
            await outboxRepository.AddAsync(outboxMessage, ct);
            await shoppingCartsRepository.ClearCartAsync(cart.Id, ct);
            await sagaStatesRepository.AddAsync(sagaState, ct);
            
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

    public async Task<string> GetStatusAsync(Guid orderId, CancellationToken ct)
    {
        //todo add customerID check
        
        var status = await ordersRepository.GetStatusAsync(orderId, ct);
        
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

    public Task<List<OrderDto>> GetHistoryAsync(Guid userId, string timePeriod, CancellationToken ct)
    {
        var (startDate, endDate) = GetDateRange(timePeriod);
        
        return ordersRepository.GetByRangeAsync(userId, startDate, endDate, ct);
    }

    private (DateTime, DateTime) GetDateRange(string timePeriod)
    {
        var today = DateTime.UtcNow;
        DateTime startDate;
        DateTime endDate;
        
        switch (timePeriod.ToLower())
        {
            case "today":
                startDate = today;
                endDate = today.AddDays(1).AddTicks(-1);
                break;
            case "week":
                var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                startDate = today.AddDays(-diff);
                endDate = startDate.AddDays(7).AddTicks(-1);
                break;
            case "month":
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = today.AddDays(1).AddTicks(-1);
                break;
            default:
                startDate = today;
                endDate = today.AddDays(1).AddTicks(-1);
                break;
        }
        
        return (startDate.ToUniversalTime(), endDate.ToUniversalTime());
    }
}