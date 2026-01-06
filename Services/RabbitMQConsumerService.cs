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
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMQConsumerService(
            IEmailSenderService emailSenderService,
            IOptions<RabbitMQSettings> options,
            ILogger<RabbitMQConsumerService> logger)
        {
            _emailSenderService = emailSenderService;
            _settings = options.Value;
            _logger = logger;
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

            cancellationToken.Register(() =>
            {
                _logger.LogInformation("Shutting down RabbitMQ connection...");
                _channel?.CloseAsync();
                _connection?.CloseAsync();
            });

            await _channel.BasicQosAsync(0, 1, false);

            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation(
                "Connected to RabbitMQ at {Host}, queue {Queue}",
                _settings.HostName,
                _settings.QueueName);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var emailMessage = JsonSerializer.Deserialize<EmailRequest>(message);

                    if (emailMessage is not null)
                        await _emailSenderService.SendEmailAsync(emailMessage);

                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }



    public class EmailRequest
    {
        public int OrderId { get; set; }
        public string TemplateId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}