using Newtonsoft.Json;

namespace MUEats.Restaurants.Application.Helpers;

public static class JsonSerializerHelper
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
    };
}