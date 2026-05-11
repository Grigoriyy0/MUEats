using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Core.Domain.Order.Entities;

public class OrderSaga
{
    public Guid CorrelationId { get; set; }
    
    public SagaState? State { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public Guid RestaurantId { get; set; }
    
    public decimal OrderTotal { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public string? LastError { get; set; }
    
    public DateTime AcknowledgeDeadline { get; set; }
    
    public DateTime? PickUpDeadline { get; set; }
}