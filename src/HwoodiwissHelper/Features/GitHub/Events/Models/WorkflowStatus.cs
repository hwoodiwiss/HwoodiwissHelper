using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

[JsonConverter(typeof(JsonStringEnumConverter<WorkflowStatus>))]
public enum WorkflowStatus
{
    Requested,
    InProgress,
    Completed,
    Queued,
    Pending,
    Waiting
}
