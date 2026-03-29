namespace MUEats.Core.Domain.Order.ValueObjects;

public enum SagaStatus
{
    Created,
    WaitingForApproval,
    Accepted,
    Preparing,
    Prepared,
    Rejected,
    Cancelled
}