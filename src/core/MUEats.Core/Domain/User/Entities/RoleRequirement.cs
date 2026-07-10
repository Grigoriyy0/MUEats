namespace MUEats.Core.Domain.User.Entities;

public class RoleRequirement
{
    public Guid Id { get; set; }
    
    public string ValueName { get; set; }
    
    public Guid RoleId { get; set; }

    public Role? Role { get; set; }
}