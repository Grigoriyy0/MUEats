namespace MUEats.Restaurants.Infrastructure.Persistence.Outbox;

public enum OutboxStatus
{
    Pending,
    Processed, 
    Failed
}