using FCG.Notification.Worker.Services;

namespace FCG.Notification.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMQConsumerService _rabbitMQConsumerService;

        public Worker(ILogger<Worker> logger, IRabbitMQConsumerService rabbitMQConsumerService)
        {
            _logger = logger;
            _rabbitMQConsumerService = rabbitMQConsumerService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started.");
            await _rabbitMQConsumerService.StartConsumingAsync(stoppingToken);
        }
    }
}
