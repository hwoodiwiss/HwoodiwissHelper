using System.Text.Json.Serialization;
using HwoodiwissHelper.Infrastructure;

namespace HwoodiwissHelper.Models.Github;

[JsonConverter(typeof(UpperSnakeCaseStringEnumConverter<AuthorAssociation>))]
public enum AuthorAssociation
{
    Collaborator,
    Contributor,
    FirstTimer,
    FirstTimeContributor,
    Mannequin,
    Member,
    None,
    Owner,
}
