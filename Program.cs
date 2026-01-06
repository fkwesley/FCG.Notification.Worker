using FCG.Notification.Worker;
using FCG.Notification.Worker.Services;
using FCG.Notification.Worker.Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.AddConsole();
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// Load configuration
builder.Configuration
    .AddEnvironmentVariables();
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)

// Log configuration sources
logger.LogInformation("Loading configuration...");
logger.LogInformation($"RabbitMQ:HostName (builder): {builder.Configuration["RabbitMQ:HostName"] ?? "Not Found"}");
logger.LogInformation($"RabbitMQ__HostName (builder): {builder.Configuration["RabbitMQ__HostName"] ?? "Not Found"}");
logger.LogInformation($"ASPNETCORE_ENVIRONMENT (builder): {builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Not Found"}");
logger.LogInformation($"DOTNET_ENVIRONMENT (builder): {builder.Configuration["DOTNET_ENVIRONMENT"] ?? "Not Found"}");

logger.LogInformation($"RabbitMQ:HostName (EnvVar): {Environment.GetEnvironmentVariable("RabbitMQ:HostName") ?? "Not Found"}");
logger.LogInformation($"RabbitMQ__HostName (EnvVar): {Environment.GetEnvironmentVariable("RabbitMQ__HostName") ?? "Not Found"}");
logger.LogInformation($"(ASPNETCORE_ENVIRONMENT (EnvVar): {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not Found"}");
logger.LogInformation($"(DOTNET_ENVIRONMENT (EnvVar): {Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Not Found"}");

// Register services
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();
