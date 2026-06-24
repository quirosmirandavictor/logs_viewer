using LogWorkerMaker.infrastructure;
using LogWorkerMaker.infrastructure.Interfaces;
using LogWorkerMaker.models;
using LogWorkerMaker.business_layer.Interfaces;
using Microsoft.Extensions.Logging;

namespace LogWorkerMaker.business_layer
{
    public class NlogFormatGenerator : ILogSimulator
    {

        private readonly ILogger<NlogFormatGenerator> _logger;
        private readonly IQueuePublisher _publisher;
        private readonly ILogFileReader _reader;

        public NlogFormatGenerator(ILogger<NlogFormatGenerator> logger,
                                   IQueuePublisher publisher,
                                   ILogFileReader reader)
        {
            _logger = logger;
            _publisher = publisher;
            _reader = reader;
        }

        public async Task LogSimulatorAsync()
        {

            _logger.LogInformation("Start simulation");

            try
            {
                int first_number = 10;

                // This will generate a DivideByZeroException when first_number reaches 0
                while (true)
                {
                    var div_result = first_number+1 / first_number;

                    _logger.LogTrace($"Div Result {first_number + 1}/{first_number} = {div_result}");
                    first_number = first_number - 1;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Example");

                await PublishNLogAsync();

                await PublishPythonLogAsync();
            }
        }

        private async Task PublishNLogAsync()
        {
            string fileName =
                $"worker-{DateTime.Now:yyyy-MM-dd}.json";

            LogEvent? lastEvent =
                _reader.GetLastEvent(fileName);

            if (lastEvent is null)
                return;

            await _publisher.PublishAsync(new LogQueueMessage
            {
                Source = "nlog",
                Event = lastEvent,
                PublishedAtUtc = DateTime.UtcNow
            });
        }

        private async Task PublishPythonLogAsync()
        {
            string fileName =
                $"python_format-{DateTime.Now:yyyy-MM-dd}.json";

            LogEvent? lastEvent =
                _reader.GetLastEvent(fileName);

            if (lastEvent is null)
                return;

            await _publisher.PublishAsync(new LogQueueMessage
            {
                Source = "python",
                Event = lastEvent,
                PublishedAtUtc = DateTime.UtcNow
            });
        }
    }
}
