using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

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
