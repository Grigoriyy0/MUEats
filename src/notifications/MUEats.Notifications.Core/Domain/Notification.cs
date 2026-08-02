using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Notifications.Core.Domain;

public class Notification
{
    private Notification(
        Guid userId,
        Guid correlationId,
        string eventData,
        string recipientInfo,
        NotificationType type)
    {
        Id = Guid.NewGuid();
        CorrelationId = correlationId;
        UserId = userId;
        EventData = eventData;
        RecipientInfo = recipientInfo;
        Type = type;
        Status = NotificationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        AttemptCount = 0;
    }

    private Notification() { }

    public Guid Id { get; private set; }
    
    public Guid CorrelationId { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public string EventData { get; private set; } = string.Empty;
    
    public string RecipientInfo { get; private set; } = string.Empty;
    
    public NotificationType Type { get; private set; }
    
    public NotificationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? SentAt { get; private set; }
    
    public DateTime? NextAttemptAt { get; private set; }
    
    public string? LastError { get; private set; }
    
    public int AttemptCount { get; private set; }
    
    public Guid? LockId { get; set; }
    
    public static Result<Notification, Error> Create(
        Guid userId,
        Guid correlationId,
        string eventData,
        string recipientInfo,
        NotificationType type)
    {
        if (string.IsNullOrWhiteSpace(eventData))
        {
            return DomainErrors.Notification.EventDataIsEmpty;
        }

        if (string.IsNullOrWhiteSpace(recipientInfo))
        {
            return DomainErrors.Notification.RecipientInfoIsEmpty;
        }

        return new Notification(userId, correlationId, eventData, recipientInfo, type);
    }

    public void MarkAsPending()
    {
        Status = NotificationStatus.Pending;
    }

    public void MarkAsSent()
    {
        SentAt = DateTime.UtcNow;
        Status = NotificationStatus.Sent;
        LastError = null;
    }

    public void RecordFailure(string error, TimeSpan retryDelay, int maxAttempts = 3)
    {
        AttemptCount++;
        LastError = error;

        if (AttemptCount >= maxAttempts)
        {
            Status = NotificationStatus.Failed;
            NextAttemptAt = null;
        }
        else
        {
            Status = NotificationStatus.Pending;
            NextAttemptAt = DateTime.UtcNow.Add(retryDelay);
        }
    }
}