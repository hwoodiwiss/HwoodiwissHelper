using System.Text.Json;
using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Infrastructure.Github;

public sealed class UppercaseStringEnumConverter<T>() : JsonStringEnumConverter<T>(JsonNamingPolicy.SnakeCaseUpper)
    where T : struct, Enum;
