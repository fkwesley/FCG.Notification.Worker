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

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting RabbitMQ consumer...");
                    await _rabbitMQConsumerService.StartConsumingAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // shutdown normal
                    _logger.LogInformation("Worker cancellation requested.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RabbitMQ connection failed. Retrying in 10 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }
        }

    }
}
