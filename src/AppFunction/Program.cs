using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()

    .ConfigureFunctionsWorkerDefaults()
    
    // 2. Carga tus configuraciones locales de Azurite
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .Build();

host.Run();