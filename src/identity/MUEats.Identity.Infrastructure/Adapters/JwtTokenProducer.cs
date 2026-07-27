using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MUEats.Identity.Application.Ports;
using MUEats.Identity.Core.Domain.Role;
using MUEats.Identity.Core.Domain.User;
using MUEats.Identity.Infrastructure.Options;

namespace MUEats.Identity.Infrastructure.Adapters;

public class JwtTokenProducer : ITokenProducer
{
    private readonly AuthOptions _options;
    private readonly RsaSecurityKey _signinKey;

    public JwtTokenProducer(IOptions<AuthOptions> options)
    {
        _options = options.Value;
        
        var rsa = RSA.Create();
        rsa.ImportFromPem(File.ReadAllText(_options.PrivateKeyPath));

        _signinKey = new RsaSecurityKey(rsa);
    }


    public string ProduceToken(User user, List<Role> roles)
    {
        var claims = GetUserClaims(user, roles);
        
        return InternalProduce(claims);
    }
    
    
    private IEnumerable<Claim> GetUserClaims(User user, List<Role> roles)
    {
        yield return new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());
        yield return new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress.Value);
    
        foreach (var userRole in roles)
        {
            if (userRole?.RoleName != null)
            {
                yield return new Claim("role", userRole.RoleName);
            }
        }
        
        foreach (var attribute in user.Attributes)
        {
            yield return new Claim(attribute.Key, attribute.Value);
        }
    }
    
    private string InternalProduce(IEnumerable<Claim> claims)
    {
        var now = DateTime.UtcNow;
        
        var expires = now.Add(TimeSpan.FromMinutes(_options.AccessTokenExpirationMinutes));
        
        var signingCredentials = new SigningCredentials(
            _signinKey, 
            SecurityAlgorithms.RsaSha256
        );

        var jwt = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}