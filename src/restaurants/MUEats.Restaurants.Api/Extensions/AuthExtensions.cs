using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MUEats.Restaurants.Api.Extensions;

public static class AuthExtensions
{
    public static void AddRsaAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var keyPath = configuration["AuthOptions:PublicKeyPath"];
                if (string.IsNullOrEmpty(keyPath) || !File.Exists(keyPath))
                {
                    throw new FileNotFoundException("RSA Public Key not found at specified path.");
                }

                var pemContent = File.ReadAllText(keyPath);
                var rsa = RSA.Create();
                rsa.ImportFromPem(pemContent);

                var rsaKey = new RsaSecurityKey(rsa);
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = rsaKey,
                    ValidateIssuer = true,
                    ValidIssuer = configuration["AuthOptions:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["AuthOptions:Audience"],
                    CryptoProviderFactory = new CryptoProviderFactory{CacheSignatureProviders = true}
                };
            });
    }
}