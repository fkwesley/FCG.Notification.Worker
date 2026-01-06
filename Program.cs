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
logger.LogInformation(
    "Configuration Details: " +
    "RabbitMQ:HostName (builder): {RabbitMQHostName}, " +
    "RabbitMQ:QueueName (builder): {RabbitMQQueueName}, " +
    "ASPNETCORE_ENVIRONMENT (builder): {AspNetCoreEnvironment}, " +
    "DOTNET_ENVIRONMENT (builder): {DotNetEnvironment}",
    builder.Configuration["RabbitMQ:HostName"] ?? "Not Found",
    builder.Configuration["RabbitMQ:QueueName"] ?? "Not Found",
    builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Not Found",
    builder.Configuration["DOTNET_ENVIRONMENT"] ?? "Not Found"
);

// Register services
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();
