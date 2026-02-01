using MUEats.Core.Domain.User.Utils;
using MUEats.Core.Primitives;

namespace MUEats.Core.Domain.User;

public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    
    public Address Address { get; set; }
    
    public Role Role { get; set; }
}