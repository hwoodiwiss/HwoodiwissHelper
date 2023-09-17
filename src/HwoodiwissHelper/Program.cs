using HwoodiwissHelper.Extensions;

var app = WebApplication
    .CreateSlimBuilder(args)
    .ConfigureAndBuild();

await app
    .ConfigureRequestPipeline()
    .RunAsync();

public partial class Program
{
    
}
