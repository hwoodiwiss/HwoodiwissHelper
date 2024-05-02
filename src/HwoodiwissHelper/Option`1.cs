using Dunet;

namespace HwoodiwissHelper;

[Union]
public partial record Option<T>
{
    public partial record Some(T Value);

    public partial record None;
}
