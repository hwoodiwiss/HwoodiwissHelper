using Dunet;

namespace HwoodiwissHelper.Core;

[Union]
public partial record Problem
{
    public partial record Exceptional(Exception Value);

    public partial record Reason(string Value);
}
