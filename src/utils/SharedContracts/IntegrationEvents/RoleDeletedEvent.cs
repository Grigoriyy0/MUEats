namespace SharedContracts.IntegrationEvents;

public class RoleDeletedEvent : IntegrationEvent
{
    public Guid RoleId { get; init; }
    
    public string RoleName { get; init; }
}