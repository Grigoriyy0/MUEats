namespace MUEats.Restaurants.Infrastructure.Persistence.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }
    
    public string Type { get; set; }
    
    public string JsonPayload { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ProcessedAt { get; set; }
    
    public int AttemptsCount { get; set; }
    
    public string? LastError { get; set; }
}