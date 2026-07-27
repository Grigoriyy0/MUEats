using System.Reflection;

namespace MUEats.Identity.Infrastructure.Utils;

internal static class ReflectionExtensions
{
    internal static T SetProperty<T, V>(this T entity, string propertyName, V value)
    {
        var type = entity!.GetType();

        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        if (property != null && property.CanWrite)
        {
            property.SetValue(entity, value);
            return entity;
        }

        throw new InvalidOperationException("Specified property is not found.");
    }
}