using System.Text.Json;

namespace CleanStart.Shared.Extensions;

public static class ObjectExtensions
{
    public static T DeepCopy<T>(this T source, JsonSerializerOptions? options = null) where T : class
    {
        var json = JsonSerializer.Serialize(source, options);
        return JsonSerializer.Deserialize<T>(json, options) ?? throw new Exception($"deepcopy deserialize result is null type={typeof(T).FullName}");
    }
}
