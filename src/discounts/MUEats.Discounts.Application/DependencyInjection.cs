using Microsoft.Extensions.DependencyInjection;
using MUEats.Discounts.Application.Services;

namespace MUEats.Discounts.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DiscountsService>();
    }
}