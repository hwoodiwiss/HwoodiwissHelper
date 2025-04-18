using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.HttpClients;

[JsonSerializable(typeof(InstallationTokenRequest))]
[JsonSerializable(typeof(InstallationTokenResponse))]
[JsonSerializable(typeof(SubmitReviewRequest))]
[JsonSerializable(typeof(AuthorizeUserRequest))]
[JsonSerializable(typeof(RefreshUserRequest))]
[JsonSerializable(typeof(AuthorizeUserResponse))]
public sealed partial class GitHubClientJsonSerializerContext : JsonSerializerContext;
