using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

[JsonConverter(typeof(JsonStringEnumConverter<MergeMethod>))]
public enum MergeMethod
{
    Merge,
    Squash,
    Rebase,
}
