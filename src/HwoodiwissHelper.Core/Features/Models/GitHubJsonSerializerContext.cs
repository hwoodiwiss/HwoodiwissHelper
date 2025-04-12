using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Core.Features.Models;

[JsonSerializable(typeof(UserAuthenticationDetails))]
public sealed partial class GitHubJsonSerializerContext : JsonSerializerContext;
