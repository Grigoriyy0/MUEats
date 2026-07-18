namespace SharedContracts.IntegrationEvents;

public class RoleUpdatedEvent : IntegrationEvent
{
    public Guid RoleId { get; init; }
    
    public string OldName { get; init; }
    
    public string NewName { get; init; }
}