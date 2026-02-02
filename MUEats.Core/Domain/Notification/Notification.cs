using CSharpFunctionalExtensions;

namespace MUEats.Core.Domain.Notification;

public class Notification
{
    public Notification(string payload, string email)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Payload = payload;
        EmailAddress = email;
    }

    public Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public string Payload { get; init; }
    
    public string EmailAddress { get; init; }

    public static Result<Notification> Create(string payload, string email)
    {
        throw new NotImplementedException();
    }
}