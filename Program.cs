using Amazon.Runtime;
using Amazon.S3;
using Disqord.Bot.Hosting;
using Disqord.Gateway;
using MiataBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

var hostBuilder = Host.CreateApplicationBuilder(args);

hostBuilder.Services.AddMemoryCache();
hostBuilder.Services.AddSingleton<HttpClient>();
hostBuilder.Services.AddScoped<AttachmentService>();

hostBuilder.Services.AddSerilog(logger =>
{
#if DEBUG
    logger.MinimumLevel.Debug();
#endif

    logger.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
    logger.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
    logger.MinimumLevel.Override("Disqord", LogEventLevel.Information);

    const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";
    
    logger
        .WriteTo.Console(outputTemplate: outputTemplate)
        .WriteTo.File("Logs/log_.txt", outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day);
});

var dbConnectionString = hostBuilder.Configuration["MIATABOT_DB_CONNECTION_STRING"];

hostBuilder.Services.AddDbContextFactory<MiataDbContext>(x => x.UseNpgsql(dbConnectionString).UseSnakeCaseNamingConvention());

var b2KeyId = hostBuilder.Configuration["MIATABOT_B2_KEYID"];
var b2Key = hostBuilder.Configuration["MIATABOT_B2_KEY"];
var b2BaseUrl = hostBuilder.Configuration["MIATABOT_B2_BASE_URL"];

hostBuilder.Services.AddSingleton(new AmazonS3Client(new BasicAWSCredentials(b2KeyId, b2Key), new AmazonS3Config { ServiceURL = b2BaseUrl }));

var botToken = hostBuilder.Configuration["MIATABOT_TOKEN"];

hostBuilder.ConfigureDiscordBot<MiataDiscordBot>(new DiscordBotHostingContext
{
    Token = botToken,
    Intents = GatewayIntents.LibraryRecommended
});

var host = hostBuilder.Build();

// I don't like this, but it makes things very convenient.
DbModelExtensions.Services = host.Services;

await host.RunAsync();