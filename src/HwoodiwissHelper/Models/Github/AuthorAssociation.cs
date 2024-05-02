using System.Text.Json.Serialization;
using HwoodiwissHelper.Infrastructure.Github;

namespace HwoodiwissHelper.Models.Github;

[JsonConverter(typeof(UppercaseStringEnumConverter<AuthorAssociation>))]
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
