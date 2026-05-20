using MUEats.Application.Dto.Order;
using MUEats.Application.IntegrationEvents;
using MUEats.Application.Interfaces;
using MUEats.Application.Ports;
using MUEats.Core.Domain.Order;
using MUEats.Core.Domain.Order.Entities;
using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Infrastructure.Adapters.Services;

public class OrdersService : IOrdersService
{
    private readonly IShoppingCartsRepository _shoppingCartsRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _uow;
    private readonly IOutboxService _outboxService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IOrderSagasRepository _sagas;
    
    public OrdersService(IShoppingCartsRepository shoppingCartsRepository,
        IOrdersRepository ordersRepository,
        IUnitOfWork uow,
        IOutboxService outboxService,
        ICurrentUserContext currentUserContext, 
        IOrderSagasRepository sagas)
    {
        _shoppingCartsRepository = shoppingCartsRepository;
        _ordersRepository = ordersRepository;
        _uow = uow;
        _outboxService = outboxService;
        _currentUserContext = currentUserContext;
        _sagas = sagas;
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

            var saga = new OrderSaga
            {
                CorrelationId = order.Id,
                State = SagaState.Created,
                AcknowledgeDeadline = DateTime.UtcNow.AddMinutes(5),
                RestaurantId = order.RestaurantId,
                CustomerId = order.UserId,
                CreatedAt = DateTime.UtcNow,
                OrderTotal =  order.TotalPrice,
            };
            
            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id,
                Dto = new OrderDto
                {
                    Id = order.Id,
                    RestaurantId = order.RestaurantId,
                    TotalPrice = order.TotalPrice,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ItemName = oi.Name,
                        Price = oi.Price,
                        Quantity = oi.Quantity
                    }).ToList()
                }
            };

            await _outboxService.CreateAsync(@event, ct);
            await _sagas.AddAsync(saga, ct);
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

    public async Task CancelAsync(Guid orderId, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        var order =  await _ordersRepository.GetByIdAsync(orderId, ct);

        if (order is null)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw new ArgumentException("Order not found");
        }
        
        var userId =  _currentUserContext.GetUserId();

        if (order.UserId != userId)
        {
            await _uow.RollbackTransactionAsync(ct);
            throw new ArgumentException("Failed to cancel");
        }
        
        var @event = new OrderCancelledEvent
        {
            OrderId = orderId,
        };
        
        await _outboxService.CreateAsync(@event, ct);
        
        await _uow.SaveChangesAsync(ct);
        await _uow.CommitTransactionAsync(ct);
    }
}