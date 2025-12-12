using Microsoft.Extensions.Hosting;

namespace OrderProcessorService.Services
{
    public class Worker : BackgroundService
    {
        private readonly FileWatcherService _watcher;
        private readonly ILogger<Worker> _logger;

        public Worker(FileWatcherService watcher, ILogger<Worker> logger)
        {
            _watcher = watcher;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Order Processor Service started.");
            _watcher.StartWatching(stoppingToken);
            return Task.CompletedTask;
        }
    }
}
