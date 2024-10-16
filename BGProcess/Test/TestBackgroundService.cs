using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BGProcess.Test
{

    public class TestBackgroundService : BackgroundService
    {
        private readonly ILogger<TestBackgroundService> _logger;

        public TestBackgroundService(ILogger<TestBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TestBackgroundService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("TestBackgroundService running...");
                await Task.Delay(1000, stoppingToken);
            }
            _logger.LogInformation("TestBackgroundService stopping.");
        }
    }
}