namespace MUEats.Core.Domain.User.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime ExpiresOn { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public bool IsRevoked { get; set; }
    
    public User? User { get; set; }
}