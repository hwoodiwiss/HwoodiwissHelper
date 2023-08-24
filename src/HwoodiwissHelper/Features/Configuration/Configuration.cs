using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace HwoodiwissHelper.Features.Configuration;

public static class Configuration
{
    public static IEndpointRouteBuilder MapConfigurationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/configuration");
        
        group.MapGet("/version", VersionConfiguration.FromMetadata);
        return builder;
    }
}
