using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessorService.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() // Enables Windows Service mode
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<FileWatcherService>();
        services.AddSingleton<OrderProcessor>();
        services.AddSingleton<DatabaseService>();
    })
    .Build();

await host.RunAsync();
