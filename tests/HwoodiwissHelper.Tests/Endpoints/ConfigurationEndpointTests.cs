using System.Text.Json.Nodes;
using System.Net;
using System.Net.Http.Json;
using HwoodiwissHelper.Tests.Integration.Assertions;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

public class ConfigurationEndpointTests(IntegrationFixture fixture) : IClassFixture<IntegrationFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task Get_Version_ReturnsApplicationMetadata()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/configuration/version");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var actualContent = await response.Content.ReadFromJsonAsync<Dictionary<string, JsonNode>>();
        actualContent.ShouldNotBeNull();
        actualContent.Keys.ShouldContainAll([
            "version",
            "gitBranch",
            "gitCommit",
            "systemArchitecture",
            "runtimeVersion",
            "aspNetCoreVersion",
            "aspNetCoreRuntimeVersion",
            "isDynamicCodeCompiled",
            "isDynamicCodeSupported",
            "isNativeAot",
        ]);
    }
}
