using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MUEats.Application.Ports;
using MUEats.Application.Responses;
using MUEats.Core.Domain.User;
using MUEats.Infrastructure.Options;

namespace MUEats.Infrastructure.Adapters.Services;

public class TokenProducer(IOptions<AuthOptions> options) : ITokenProducer
{
    private readonly AuthOptions _options = options.Value;
    
    public string ProduceToken(User user)
    {
        var claims = GetClaims(user);

        return InternalProduce(claims);
    }

    public string ProduceRefreshToken()
    {
        var rnd = new byte[64];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(rnd);

        return Convert.ToBase64String(rnd);
    }

    public TokenResponse ProduceTokenPair(User user)
    {
        var accessToken = ProduceToken(user);

        var refreshToken = ProduceRefreshToken();
        
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private static IEnumerable<Claim> GetClaims(User user)
    {
        yield return new Claim("id", user.Id.ToString());
        yield return new Claim("email", user.Email);
        yield return new Claim("role", user.Role.ToString());
    }

    private string InternalProduce(IEnumerable<Claim> claims)
    {
        var now = DateTime.UtcNow;
        
        var expires = now.Add(TimeSpan.FromMinutes(_options.AccessTokenExpirationMinutes));

        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));
        
        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa), 
            SecurityAlgorithms.RsaSha256
        );

        var jwt = new JwtSecurityToken(
            options.Value.Issuer,
            options.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}