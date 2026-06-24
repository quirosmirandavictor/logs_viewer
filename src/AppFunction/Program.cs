using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using AppFunction.Services;
using AppFunction.Services.Interfaces;

var host = new HostBuilder()

    .ConfigureFunctionsWorkerDefaults()
    
    // 2. Carga tus configuraciones locales de Azurite
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Storage");

        services.AddSingleton(_ =>
            new TableServiceClient(connectionString));

        services.AddSingleton<ITableStorageService, TableStorageService>();
        services.AddSingleton<ILogMessageProcessor, LogMessageProcessor>();
    })
    .Build();

host.Run();