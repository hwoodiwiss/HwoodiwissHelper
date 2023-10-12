using System.Net;
using System.Net.Http.Json;
using HwoodiwissHelper.Tests.Integration.Assertions;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

public class ConfigurationEndpointTests(HwoodiwissHelperFixture fixture) : IClassFixture<HwoodiwissHelperFixture>
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
        var actualContent = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        actualContent.ShouldNotBeNull();
        actualContent.Keys.ShouldContainAll([
            "isNativeAot",
            "version",
            "gitBranch",
            "gitCommit",
            "systemArchitecture",
            "runtimeVersion",
            "aspNetCoreVersion",
            "aspNetCoreRuntimeVersion"
        ]);
    }
}
