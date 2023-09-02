namespace HwoodiwissHelper.Infrastructure;

public interface IGithubSignatureValidator
{
    ValueTask<bool> ValidateSignature(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken);
}
