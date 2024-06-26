﻿using System.Net;

namespace HwoodiwissHelper.Tests.Integration.Endpoints;

public class HealthEndpointTests(HwoodiwissHelperFixture fixture) : IClassFixture<HwoodiwissHelperFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

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
