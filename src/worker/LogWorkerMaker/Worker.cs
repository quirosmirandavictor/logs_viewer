using LogWorkerMaker.business_layer;

namespace LogWorkerMaker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly NlogFormatGenerator _log_simulator;

        public Worker(ILogger<Worker> logger, NlogFormatGenerator procesador)
        {
            _logger = logger;
            _log_simulator = procesador;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Log Worker Maker is running !");

                _log_simulator.log_simulator();

                await Task.Delay(15000, stoppingToken);
            }
        }
    }
}
