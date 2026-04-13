using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MUEats.Infrastructure.Handlers;

namespace MUEats.Infrastructure;

public static class HandlersDependencyInjection
{
    public static void AddIntegrationEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                .Select(i => new { HandlerType = t, InterfaceType = i }))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            services.AddScoped(handlerType.InterfaceType, handlerType.HandlerType);
        }
    }
}