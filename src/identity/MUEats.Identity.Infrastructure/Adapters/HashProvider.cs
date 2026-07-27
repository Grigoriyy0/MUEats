using MUEats.Identity.Application.Ports;

namespace MUEats.Identity.Infrastructure.Adapters;

public class HashProvider : IHashProvider
{
    public string ComputeHash(string input)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(input, 12);
    }

    public bool VerifyHash(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}