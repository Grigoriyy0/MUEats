namespace MUEats.Core.Domain.Notification;

public class Notification
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Payload { get; init; } = null!;

    public string EmailAddress { get; init; } = null!;
}