
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

public static class JsonSerializerOptionsExtensions
{
    public static JsonTypeInfo<T> GetJsonTypeInfo<T>(this JsonSerializerOptions options) =>
        options.GetTypeInfo(typeof(T)) is JsonTypeInfo<T> jsonTypeInfo
            ? jsonTypeInfo
            : throw new ArgumentException($"Unable to find JsonTypeInfo for {typeof(T).FullName}");

    public static string Serialize<T>(this JsonSerializerOptions options, T value)
    {
        JsonTypeInfo<T> jti = options.GetJsonTypeInfo<T>();
        return JsonSerializer.Serialize(value, jti);
    }
}
