using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

[JsonConverter(typeof(JsonStringEnumConverter<WorkflowConclusion>))]
public enum WorkflowConclusion
{
    Success,
    Failure,
    Neutral,
    Cancelled,
    TimesOut,
    ActionRequired,
    Stale,
    Null,
    Skipped
}
