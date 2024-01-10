using System.Text.Json;
using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Infrastructure;

public sealed class UpperSnakeCaseStringEnumConverter<T>() : JsonStringEnumConverter<T>(JsonNamingPolicy.SnakeCaseUpper)
    where T : struct, Enum;
