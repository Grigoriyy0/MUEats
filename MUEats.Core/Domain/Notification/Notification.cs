namespace MUEats.Core.Domain.Notification;

public class Notification
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string Payload { get; set; }
}