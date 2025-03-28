using Dunet;

namespace HwoodiwissHelper.Core;

[Union]
public partial record Result<TResult, TError>
{
    public partial record Success(TResult Value);

    public partial record Failure(TError Error);
}
