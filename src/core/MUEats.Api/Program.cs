using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MUEats.Application;
using MUEats.Extensions;
using MUEats.Infrastructure;
using MUEats.Infrastructure.Persistence;
using Prometheus;

namespace MUEats;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddRsaAuth(builder.Configuration);
        
        builder.Services.AddAuthorization(opt =>
        {
            opt.AddPolicy("Customer", policy => 
                policy.RequireRole("Customer"));
            
            opt.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));
            
            opt.AddPolicy("RestaurantManager",  policy =>
                policy.RequireRole("RestaurantManager"));
        });

        
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
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
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync();
        }
        
        app.UseHttpsRedirection();

        app.UseHttpMetrics();
        app.MapMetrics();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}