using LogWorkerMaker.business_layer;
using LogWorkerMaker.business_layer.Interfaces;

namespace LogWorkerMaker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILogSimulator _log_simulator;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, ILogSimulator procesador, IConfiguration configuration)
        {
            _logger = logger;
            _log_simulator = procesador;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Log Worker Maker is running !");

                await _log_simulator.LogSimulatorAsync();

                int delaySeconds = _configuration.GetValue<int?>("DelaySeconds") ?? 120;

                await Task.Delay(delaySeconds * 1000, stoppingToken);
            }
        }
    }
}
