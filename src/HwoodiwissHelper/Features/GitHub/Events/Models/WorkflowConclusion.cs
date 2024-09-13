using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

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
