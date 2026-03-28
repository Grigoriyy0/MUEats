namespace MUEats.Core.Domain.Order.ValueObjects;

public enum SagaStatus
{
    Created,
    WaitingForApproval,
    Approved,
    Rejected,
    Cancelled
}