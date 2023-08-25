using HwoodiwissHelper;
using HwoodiwissHelper.Extensions;
using HwoodiwissHelper.Features.Configuration;

using Microsoft.AspNetCore.Cors.Infrastructure;

var app = WebApplication
    .CreateSlimBuilder(args)
    .ConfigureAndBuild();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() && !ApplicationMetadata.IsNativeAot)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapGet("/heartbeat", () => Results.Ok())
    .WithDescription("Gets a heartbeat to check if the service is running.");

app.MapEndpoints();

app.Run();
