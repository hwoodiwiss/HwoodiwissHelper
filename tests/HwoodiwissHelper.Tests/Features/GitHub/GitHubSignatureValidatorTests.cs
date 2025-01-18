using HwoodiwissHelper.Configuration;
using HwoodiwissHelper.Features.GitHub.Configuration;
using HwoodiwissHelper.Features.GitHub.Services;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace HwoodiwissHelper.Tests.Unit.Infrastructure;

public class GitHubSignatureValidatorTests
{
    /// <summary>
    /// This test case is taken from the GitHub documentation.
    /// https://docs.github.com/en/webhooks/using-webhooks/securing-your-webhooks#validating-payloads-from-github
    /// </summary>
    [Fact]
    public async Task ValidateSignature_ValidSignature_ReturnsTrue()
    {
        // Arrange
        var signature = "757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17";
        var body = new MemoryStream("Hello, World!"u8.ToArray());
        var cancellationToken = CancellationToken.None;
        var sut = CreateSut("It's a Secret to Everybody");

        // Act
        var result = await sut.ValidateSignatureAsync(signature.AsMemory(), body, cancellationToken);

        // Assert
        result.ShouldBeTrue();
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData("not my signature")]
    public async Task ValidateSignature_InvalidSignature_ReturnsFalse(string signature)
    {
        // Arrange
        var body = new MemoryStream("Hello, World!"u8.ToArray());
        var cancellationToken = CancellationToken.None;
        var sut = CreateSut("It's a Secret to Everybody");

        // Act
        var result = await sut.ValidateSignatureAsync(signature.AsMemory(), body, cancellationToken);

        // Assert
        result.ShouldBeFalse();
    }

    private static GitHubSignatureValidator CreateSut(string key)
    {
        var mockOptionsMonitor = Substitute.For<IOptionsMonitor<GitHubConfiguration>>();
        mockOptionsMonitor.CurrentValue.Returns(new GitHubConfiguration
        {
            WebhookKey = key,
            EnableRequestLogging = false,
            AppId = string.Empty,
            AppPrivateKey = string.Empty,
            AllowedBots = [],
            ClientId = string.Empty,
            ClientSecret = string.Empty,
            AllowedRedirectHosts = []
        });

        return new GitHubSignatureValidator(mockOptionsMonitor);
    }
}
