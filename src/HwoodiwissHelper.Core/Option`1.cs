using Dunet;

namespace HwoodiwissHelper.Core;

[Union]
public partial record Option<T>
{
    public partial record Some(T Value);

    public partial record None;
}
