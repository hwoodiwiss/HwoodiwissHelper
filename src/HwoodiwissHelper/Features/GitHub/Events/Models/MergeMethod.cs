using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

[JsonConverter(typeof(JsonStringEnumConverter<MergeMethod>))]
public enum MergeMethod
{
    Merge,
    Squash,
    Rebase,
}
