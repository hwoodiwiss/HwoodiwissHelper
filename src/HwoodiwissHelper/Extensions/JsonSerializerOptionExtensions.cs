using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace HwoodiwissHelper.Extensions;

public static class JsonSerializerOptionExtensions
{
    public static JsonTypeInfo<T> GetJsonTypeInfo<T>(this JsonSerializerOptions options)
    {
        if (options.TryGetTypeInfo(typeof(T), out var jsonTypeInfo) &&
            jsonTypeInfo is JsonTypeInfo<T> info)
        {
            return info;
        }

        throw new ArgumentException($"Unable to find JsonTypeInfo for {typeof(T).FullName}");
    }
}
