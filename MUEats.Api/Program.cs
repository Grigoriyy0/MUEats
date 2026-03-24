using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MUEats.Application;
using MUEats.Infrastructure;
using MUEats.Infrastructure.Persistence;

namespace MUEats;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["AuthOptions:Issuer"],
                    ValidAudience = builder.Configuration["AuthOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["AuthOptions:Key"]!)
                    )
                };
            });
        
        builder.Services.AddAuthorization();

        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        
        
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MueDbContext>();
            dbContext.Database.Migrate();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}