using System.Security.Cryptography;
using System.Text;
using MUEats.Application.Ports;

namespace MUEats.Infrastructure.Adapters.Services;

public class HashProvider : IHashProvider
{
    public string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

        var sb = new StringBuilder();

        foreach (var t in bytes)
        {
            sb.Append(t.ToString("x2"));
        }

        return sb.ToString();
    }
}