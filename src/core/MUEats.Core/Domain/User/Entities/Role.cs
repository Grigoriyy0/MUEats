namespace MUEats.Core.Domain.User.Entities;

public class Role
{
    public Guid Id { get; set; }
    
    public string RoleName { get; set; }

    public ICollection<UserRole> Roles { get; set; } = [];
}