using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Adapters.Services;

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