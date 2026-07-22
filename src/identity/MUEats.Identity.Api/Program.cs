using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MUEats.Identity.Application;
using MUEats.Identity.Infrastructure;
using MUEats.Identity.Infrastructure.Persistence;

namespace MUEats.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                
                var keyPath = builder.Configuration["AuthOptions:PrivateKeyPath"];
                if (string.IsNullOrEmpty(keyPath) || !File.Exists(keyPath))
                {
                    throw new FileNotFoundException("RSA Private Key is not found at specified path.");
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
                    ValidIssuer = builder.Configuration["AuthOptions:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["AuthOptions:Audience"],
                    CryptoProviderFactory = new CryptoProviderFactory{CacheSignatureProviders = true},
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                    RoleClaimType = "role", 
                    NameClaimType = "sub"
                };
            });
        
        builder.Services.AddSwaggerGen(options =>
        {
            const string securitySchemeId = "bearer"; 
    
            options.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",          
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme. Enter your token below."
            });
    
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeId, document)] = []
            });
        });
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using var scope = app.Services.CreateScope();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

        seeder.SeedAsync().Wait();
        
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}