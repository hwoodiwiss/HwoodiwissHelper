using System.Text.Json.Serialization;
using HwoodiwissHelper.UI.Features.GitHubForkCleaner.Models;

namespace HwoodiwissHelper.UI.Features.GitHubForkCleaner.HttpClients;

[JsonSerializable(typeof(Repository))]
[JsonSerializable(typeof(Repository[]))]
[JsonSerializable(typeof(User))]
public sealed partial class GitHubClientJsonSerializerContext : JsonSerializerContext;
