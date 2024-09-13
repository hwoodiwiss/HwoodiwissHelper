using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Features.GitHub.Events.Models;

[JsonConverter(typeof(JsonStringEnumConverter<ActorType>))]
public enum ActorType
{
    Bot,
    User,
    Organization
}
