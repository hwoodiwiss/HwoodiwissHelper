using System.Text.Json.Serialization;

namespace HwoodiwissHelper.Models.Github;

[JsonConverter(typeof(JsonStringEnumConverter<ActorType>))]
public enum ActorType
{
    Bot,
    User,
    Organization
}
