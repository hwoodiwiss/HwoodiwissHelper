using HwoodiwissHelper;
using HwoodiwissHelper.Extensions;

var app = WebApplication
    .CreateSlimBuilder(args)
    .ConfigureAndBuild();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() && !ApplicationMetadata.IsNativeAot)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/heartbeat", () => Results.Ok())
    .WithDescription("Gets a heartbeat to check if the service is running.");

app.MapEndpoints(app.Environment);

app.Run();


public partial class Program
{
    
}
