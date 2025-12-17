using FCG.Notification.Worker;
using FCG.Notification.Worker.Services;
using FCG.Notification.Worker.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

// Register services
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IRabbitMQConsumerService, RabbitMQConsumerService>();
builder.Services.AddSingleton<IEmailSenderService, EmailSenderService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
