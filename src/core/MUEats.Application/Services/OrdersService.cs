using MUEats.Application.Dto.Order;
using MUEats.Application.Ports;
using MUEats.Application.Queries;
using MUEats.Core.Domain.Events.Order;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Application.Services;

public class OrdersService(IShoppingCartsRepository shoppingCartsRepository,
    IOrdersRepository ordersRepository,
    IUnitOfWork uow,
    IOutboxService outboxService,
    ICurrentUserContext currentUserContext)
{
    public async Task<Guid> CreateAsync(CreateOrderDto dto, 
        CancellationToken ct)
    {
        try
        {
            if (dto.PickUpTime <= DateTime.UtcNow.AddMinutes(10) && dto.PickUpTime is not null)
            {
                throw new ArgumentException("Pick up time is incorrect");
            }
            
            await uow.BeginTransactionAsync(ct);

            var userId = currentUserContext.GetUserId();
            
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
}