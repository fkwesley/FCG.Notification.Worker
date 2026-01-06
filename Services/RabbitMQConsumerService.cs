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
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQConsumerService(
            IEmailSenderService emailSenderService,
            IOptions<RabbitMQSettings> options)
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

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(null, cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var emailMessage = JsonSerializer.Deserialize<EmailRequest>(message);

                    if (emailMessage is not null)
                    {
                        await _emailSenderService.SendEmailAsync(emailMessage);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex}");

                    // Requeue controlado (pode virar DLQ depois)
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);

            // Mantém o consumer vivo
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        public async Task StopAsync()
        {
            if (_channel is not null)
                await _channel.CloseAsync();

            if (_connection is not null)
                await _connection.CloseAsync();
        }
    }


    public class EmailRequest
    {
        public string OrderId { get; set; }
        public string TemplateId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}