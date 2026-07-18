using MUEats.Discounts.Api.Extensions;
using MUEats.Discounts.Api.Utils;
using MUEats.Discounts.Application;
using MUEats.Discounts.Infrastructure;

namespace MUEats.Discounts.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerExtensions();
        builder.Services.AddRsaAuth(builder.Configuration);
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<CurrentUserContext>();
        
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}