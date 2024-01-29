using Dunet;

namespace HwoodiwissHelper;

[Union]
public partial record Maybe<T>
{
    public partial record Some(T Value);

    public partial record None;
}
