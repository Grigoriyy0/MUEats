using Newtonsoft.Json;

namespace MUEats.Application.Helpers;

public static class JsonSerializerHelper
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        Formatting = Formatting.Indented
    };
}