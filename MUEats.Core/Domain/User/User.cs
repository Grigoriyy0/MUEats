using CSharpFunctionalExtensions;
using MUEats.Core.Domain.User.Utils;
using MUEats.Core.Primitives;
using MUEats.Core.Primitives.ValueObjects;

namespace MUEats.Core.Domain.User;

public class User
{
    public User()
    {
        
    }
    
    private User(string username, string firstName, string lastName, string email, string passwordHash, Address address, Role role)
    {
        Id = Guid.NewGuid();
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Address = address;
        Role = role;
    }

    public Guid Id { get; init; }
    
    public string Username { get; private set; }
    
    public string FirstName { get; private set; }
    
    public string LastName { get; private set; }
    
    public string Email { get; private set; }
    
    public string PasswordHash { get; private set; }
    
    public Address Address { get; private set; }
    
    public Role Role { get; private set; }

    public static Result<User> Create(string username, string firstName, string lastName, string email, string passwordHash, Address address, Role role)
    {
        return new User(username, firstName, lastName, email, passwordHash, address, role);
    }
}