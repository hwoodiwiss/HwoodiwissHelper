namespace HwoodiwissHelper.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfigureAndBuild(this WebApplicationBuilder builder)
    {
        ConfigureConfiguration(builder.Configuration);
        ConfigureServices(builder.Services);

        return builder.Build();
    }
    
    public static IConfigurationBuilder ConfigureConfiguration(this IConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .AddJsonFile("appsettings.Secrets.json", true);
    
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, ApplicationJsonContext.Default);
        });

        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        if (!ApplicationMetadata.IsNativeAot)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        return services;
    }
}
