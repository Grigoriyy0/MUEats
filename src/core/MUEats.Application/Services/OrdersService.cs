using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Services;

public class OrdersService(IShoppingCartsRepository shoppingCartsRepository,
    IOrdersRepository ordersRepository,
    IUnitOfWork uow,
    IOutboxService outboxService)
{
    public async Task<Guid> CreateAsync(Guid userId, 
        CreateOrderDto dto, 
        CancellationToken ct)
    {
        try
        {
            await uow.BeginTransactionAsync(ct);

            if (dto.PickUpTime <= DateTime.UtcNow && dto.PickUpTime is not null)
            {
                throw new ArgumentException("Pick up time is incorrect");
            }
            
            var cart = await shoppingCartsRepository.GetCartDtoAsync(userId, ct);

            if (cart is null || cart.Items.Count == 0)
            {
                throw new ArgumentException("Add items to create an order.");
            }

            var totalPrice = cart.Items.Sum(x => x.Price);
        
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderItems = cart.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = i.FoodItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                TotalPrice = totalPrice,
                PickupTime = dto.PickUpTime,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                RestaurantId = cart.RestaurantId,
            };

            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id,
            };

            await outboxService.CreateAsync(@event, ct);
            
            await ordersRepository.AddAsync(order, ct);
            await shoppingCartsRepository.ClearCartAsync(cart.Id, ct);
            
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