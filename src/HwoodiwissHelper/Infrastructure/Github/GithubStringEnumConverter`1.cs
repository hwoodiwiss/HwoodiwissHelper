using System.Text.Json;
using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Infrastructure.Github;

public sealed class GithubStringEnumConverter<T>() : JsonStringEnumConverter<T>(JsonNamingPolicy.SnakeCaseUpper)
    where T : struct, Enum;
