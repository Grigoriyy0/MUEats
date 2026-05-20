namespace MUEats.Core;

public class InboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; }
    
    public string JsonPayload { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int RetryCount { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public string? LastError { get; set; }
    
    public Guid? LockId { get; set; }

    public InboxStatus Status { get; set; }
}