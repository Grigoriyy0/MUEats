using MUEats.Application.Dto.Order;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrdersService : IOrdersService
{
    private readonly IShoppingCartsRepository _shoppingCartsRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _uow;
    private readonly IOutboxService _outboxService;
    private readonly ICurrentUserContext _currentUserContext;

    public OrdersService(IShoppingCartsRepository shoppingCartsRepository,
        IOrdersRepository ordersRepository,
        IUnitOfWork uow,
        IOutboxService outboxService,
        ICurrentUserContext currentUserContext)
    {
        _shoppingCartsRepository = shoppingCartsRepository;
        _ordersRepository = ordersRepository;
        _uow = uow;
        _outboxService = outboxService;
        _currentUserContext = currentUserContext;
    }

    public async Task<Guid> CreateAsync(CreateOrderDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.PickUpTime <= DateTime.UtcNow.AddMinutes(10) && dto.PickUpTime is not null)
            {
                throw new ArgumentException("Pick up time is incorrect");
            }
            
            await _uow.BeginTransactionAsync(ct);

            var userId = _currentUserContext.GetUserId();
            
            var cart = await _shoppingCartsRepository.GetCartDtoAsync(userId, ct);

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

            await _outboxService.CreateAsync(@event, ct);
            
            await _ordersRepository.AddAsync(order, ct);
            //await _shoppingCartsRepository.ClearCartAsync(cart.Id, ct);
            
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
        
            return order.Id;
        }
        catch (Exception)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}