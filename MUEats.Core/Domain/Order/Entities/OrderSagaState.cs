using MUEats.Core.Domain.Order.ValueObjects;

namespace MUEats.Core.Domain.Order.Entities;

public class OrderSagaState
{
    public Guid CorrelationId { get; set; }
    
    public SagaStatus? State { get; set; }
}