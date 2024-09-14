namespace HwoodiwissHelper.Features.GitHub.Services;

public interface IGitHubSignatureValidator
{
    ValueTask<bool> ValidateSignatureAsync(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken);
}
