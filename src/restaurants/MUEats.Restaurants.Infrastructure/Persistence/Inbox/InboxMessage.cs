namespace MUEats.Restaurants.Infrastructure.Persistence.Inbox;

public class InboxMessage
{
    public Guid Id { get; set; }
    
    public string Type { get; set; }
    
    public string JsonPayload { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ProcessedAt { get; set; }
    
    public DateTime? NextAttemptAt { get; set; }
    
    public Guid? LockId { get; set; }
    
    public InboxStatus Status { get; set; }
    
    public int AttemptsCount { get; set; }
    
    public string? LastError { get; set; }
}