namespace MUEats.Infrastructure.Options;

public class AuthOptions
{
    public string Key { get; set; }
    
    public string Issuer { get; set; }
    
    public string Audience { get; set; }
    
    public int AccessTokenExpirationMinutes { get; set; }
    
    public int RefreshTokenExpirationDays { get; set; }
    
    public string PrivateKeyPath { get; set; }
}