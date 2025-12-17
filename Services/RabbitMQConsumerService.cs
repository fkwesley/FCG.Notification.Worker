using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using FCG.Notification.Worker.Infrastructure.Configurations;

namespace FCG.Notification.Worker.Services
{
    public interface IRabbitMQConsumerService
    {
        Task StartConsumingAsync(CancellationToken cancellationToken);
    }

    public class RabbitMQConsumerService : IRabbitMQConsumerService
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly RabbitMQSettings _settings;

        public RabbitMQConsumerService(IEmailSenderService emailSenderService, IOptions<RabbitMQSettings> options)
        {
            _emailSenderService = emailSenderService;
            _settings = options.Value;
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            // Fix: Pass null for CreateChannelOptions as the first argument to CreateChannelAsync
            using var connection = await factory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(null, cancellationToken);

            await channel.QueueDeclareAsync(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var emailMessage = JsonSerializer.Deserialize<EmailRequest>(message);

                    if (emailMessage != null)
                    {
                        await _emailSenderService.SendEmailAsync(emailMessage);
                    }

                    // Acknowledge the message only if processing succeeds
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    Console.WriteLine($"Error processing message: {ex.Message}");

                    // Reject the message and requeue it
                    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await channel.BasicConsumeAsync(queue: _settings.QueueName, autoAck: false, consumer: consumer);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public class EmailRequest
    {
        public string OrderId { get; set; }
        public string TemplateId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}