using System.Text.Json.Serialization;
using HwoodiwissHelper.Features.GitHub.Services;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

[JsonConverter(typeof(JsonStringEnumConverter<AuthorAssociation>))]
public enum AuthorAssociation
{
    [JsonStringEnumMemberName("COLLABORATOR")]
    Collaborator,
    [JsonStringEnumMemberName("CONTRIBUTOR")]
    Contributor,
    [JsonStringEnumMemberName("FIRSTTIMER")]
    FirstTimer,
    [JsonStringEnumMemberName("FIRSTTIMECONTRIBUTOR")]
    FirstTimeContributor,
    [JsonStringEnumMemberName("MANNEQUIN")]
    Mannequin,
    [JsonStringEnumMemberName("MEMBER")]
    Member,
    [JsonStringEnumMemberName("NONE")]
    None,
    [JsonStringEnumMemberName("OWNER")]
    Owner,
}
