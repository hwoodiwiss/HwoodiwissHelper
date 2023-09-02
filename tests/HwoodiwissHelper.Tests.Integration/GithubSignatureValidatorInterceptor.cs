using HwoodiwissHelper.Infrastructure;
using NSubsititute;

namespace HwoodiwissHelper.Tests.Integration;

public class GithubSignatureValidatorInterceptor : IGithubSignatureValidator
{
    private IGithubSignatureValidator? _innerValidator;
    private IGithubSignatureValidator validatorMock = Substitute.For<IGithubSignatureValidator>();
    
    public void UseValidator(IGithubSignatureValidator validator)
    {
        _innerValidator = validator;
    }
    
    public ValueTask<bool> ValidateSignature(ReadOnlyMemory<char> signature, Stream body, CancellationToken cancellationToken)
        => GetInnerValidator().ValidateSignature(signature, body, cancellationToken);
    
    private IGithubSignatureValidator GetInnerValidator()
        => _innerValidator ?? throw new InvalidOperationException("No validator has been configured.");
}
