namespace MUEats.Core.Domain.User.Entities;

public class UserAttribute
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Key { get; set; }
    
    public string Value { get; set; }
}