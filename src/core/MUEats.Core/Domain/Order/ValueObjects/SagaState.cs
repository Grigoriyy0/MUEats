namespace MUEats.Core.Domain.Order.ValueObjects;

public enum SagaState
{
    Created,
    Accepted,
    Prepared,
    Completed,
    Failed
}