using CSharpFunctionalExtensions;
using Primitives;

namespace MUEats.Notifications.Core.Domain;

public class Notification
{
    private Notification(Guid userId,
        string payload,
        string recipientInfo,
        NotificationType type)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Payload = payload;
        RecipientInfo = recipientInfo;
        Type = type;
        Status = NotificationStatus.Created;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public string Payload { get; private set; }
    
    public string RecipientInfo { get; private set; }
    
    public NotificationType Type { get; private set; }
    
    public NotificationStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? SentAt { get; private set; }

    public static Result<Notification, Error> Create(Guid userId,
        string payload,
        string recipientInfo,
        NotificationType type)
    {
        if (string.IsNullOrWhiteSpace(payload))
        {
            return DomainErrors.Notification.BodyIsEmpty;
        }

        if (string.IsNullOrWhiteSpace(recipientInfo))
        {
            return DomainErrors.Notification.RecipientInfoIsEmpty;
        }

        return new Notification(userId, payload, recipientInfo, type);
    }

    public void MarkAsSent()
    {
        if (SentAt is not null || Status == NotificationStatus.Sent)
        {
            return;
        }

        SentAt = DateTime.UtcNow;
        Status = NotificationStatus.Sent;
    }

    public void MarkAsFailed()
    {
        if (Status == NotificationStatus.Failed)
        {
            return;
        }

        Status = NotificationStatus.Failed;
    }
}