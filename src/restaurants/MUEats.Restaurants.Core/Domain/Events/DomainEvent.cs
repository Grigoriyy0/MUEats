namespace MUEats.Restaurants.Core.Domain.Events;

public abstract class DomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public DateTime OccuredOn { get; init; } = DateTime.UtcNow;
}