using LogWorkerMaker;
using LogWorkerMaker.business_layer;
using LogWorkerMaker.infrastructure;
using NLog;
using NLog.Extensions.Hosting;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Debug("Starting Worker Maker Service...");

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.ClearProviders();
    builder.UseNLog();

    builder.Services.AddTransient<NlogFormatGenerator>();
    builder.Services.AddSingleton<QueuePublisher>();
    builder.Services.AddSingleton<LogFileReader>();
    builder.Services.AddHostedService<Worker>();


    var host = builder.Build();
    host.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Worker Maker Service stopped because of an unhandled exception.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
