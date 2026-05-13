namespace MUEats.Restaurants.Infrastructure.ExternalServices.IntegrationEvents;

public abstract class IntegrationEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime OccuredOn { get; set; } = DateTime.UtcNow;
}