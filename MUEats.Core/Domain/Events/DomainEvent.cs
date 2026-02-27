namespace MUEats.Core.Domain.Events;

public class DomainEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime OccuredOn { get; set; } = DateTime.UtcNow;
}