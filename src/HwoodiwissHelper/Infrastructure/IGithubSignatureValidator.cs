namespace HwoodiwissHelper.Infrastructure;

public interface IGithubSignatureValidator
{
    ValueTask<bool> ValidateSignatureAsync(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken);
}
