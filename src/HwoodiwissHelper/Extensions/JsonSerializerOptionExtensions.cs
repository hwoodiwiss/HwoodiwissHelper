using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace HwoodiwissHelper.Extensions;

public static class JsonSerializerOptionExtensions
{
    public static JsonTypeInfo<T> GetJsonTypeInfo<T>(this JsonSerializerOptions options)
    {
        if (options.GetTypeInfo(typeof(T)) is JsonTypeInfo<T> jsonTypeInfo)
        {
            return jsonTypeInfo;
        }

        throw new ArgumentException($"Unable to find JsonTypeInfo for {typeof(T).FullName}");
    }
}
