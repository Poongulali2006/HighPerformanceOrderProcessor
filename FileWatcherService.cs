using System.Collections.Concurrent;

namespace OrderProcessorService.Services
{
    public class FileWatcherService
    {
        private readonly ILogger<FileWatcherService> _logger;
        private readonly OrderProcessor _processor;
        private readonly ConcurrentDictionary<string, bool> _processed = new();
        private readonly string Folder = "IncomingOrders";

        public FileWatcherService(ILogger<FileWatcherService> logger, OrderProcessor processor)
        {
            _logger = logger;
            _processor = processor;

            if (!Directory.Exists(Folder))
                Directory.CreateDirectory(Folder);
        }

        public void StartWatching(CancellationToken token)
        {
            FileSystemWatcher watcher = new()
            {
                Path = Folder,
                Filter = "*.json",
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size
            };

            watcher.Created += async (_, e) =>
            {
                await Task.Delay(500); // Wait for file unlock

                if (_processed.ContainsKey(e.FullPath))
                    return;

                _processed[e.FullPath] = true;

                _logger.LogInformation("New file detected: {file}", e.Name);

                await _processor.ProcessFile(e.FullPath);
            };
        }
    }
}
