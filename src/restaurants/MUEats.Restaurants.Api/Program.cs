using Microsoft.EntityFrameworkCore;
using MUEats.Restaurants.Api.Adapters.Realtime;
using MUEats.Restaurants.Api.Extensions;
using MUEats.Restaurants.Application;
using MUEats.Restaurants.Application.Ports;
using MUEats.Restaurants.Infrastructure;
using MUEats.Restaurants.Infrastructure.Persistence.Contexts;

namespace MUEats.Restaurants.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerExtensions();
        
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        builder.Services.AddScoped<IRealtimeDispatcher, RealtimeDispatcher>();
        
        
        builder.Services.AddRsaAuth(builder.Configuration);
        builder.Services.AddSignalR()
            .AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis")!);
        
        builder.Services.AddAuthorization(opt =>
        {
            opt.AddPolicy("Customer", policy => 
                policy.RequireRole("Customer"));
            
            opt.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));
            
            opt.AddPolicy("RestaurantManager",  policy =>
                policy.RequireRole("RestaurantManager"));
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantsDbContext>();
            dbContext.Database.Migrate();
        }
        
        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseAuthorization();
        app.MapHub<OrdersHub>("/hub/orders");
        
        app.Run();
    }
}