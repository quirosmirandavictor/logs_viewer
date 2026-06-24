using LogWorkerMaker;
using LogWorkerMaker.business_layer;
using LogWorkerMaker.business_layer.Interfaces;
using LogWorkerMaker.infrastructure;
using LogWorkerMaker.infrastructure.Interfaces;
using NLog;
using NLog.Extensions.Hosting;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Debug("Starting Worker Maker Service...");

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Logging.ClearProviders();
    builder.UseNLog();

    builder.Services.AddTransient<ILogSimulator, NlogFormatGenerator>();
    builder.Services.AddSingleton<IQueuePublisher, QueuePublisher>();
    builder.Services.AddSingleton<ILogFileReader, LogFileReader>();
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
