using System.Net;

namespace HwoodiwissHelper.Tests.Integration.Features.Health;

public class HealthEndpointTests : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(HwoodiwissHelperFixture fixture)
    {
        _client = fixture.CreateClient();
    }
    
    [Fact]
    public async Task Get_Health_ReturnsOk()
    {
        // Arrange
        
        // Act
        var response = await _client.GetAsync("/health");
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
