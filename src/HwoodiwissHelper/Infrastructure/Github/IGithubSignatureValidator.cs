namespace HwoodiwissHelper.Infrastructure.Github;

public interface IGithubSignatureValidator
{
    ValueTask<bool> ValidateSignatureAsync(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken);
}
