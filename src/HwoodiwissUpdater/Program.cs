using HwoodiwissUpdater;

#pragma warning disable CA1852

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
if (!ApplicationMetadata.IsNativeAot)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() && !ApplicationMetadata.IsNativeAot)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/heartbeat", () => Results.Ok())
    .WithDescription("Gets a heartbeat to check if the service is running.");

app.Run();
