using Dunet;

namespace HwoodiwissHelper.Core.Features.GitHub;

[Union]
public partial record GitHubError
{
    public partial record Unauthorized();
    public partial record UnexpectedResponse(string Message, string Content);
    public partial record Unknown;
}
