using LogWorkerMaker.infrastructure;
using LogWorkerMaker.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogWorkerMaker.business_layer
{
    public class NlogFormatGenerator
    {

        private readonly ILogger<NlogFormatGenerator> _logger;
        private readonly QueuePublisher _publisher;
        private readonly LogFileReader _reader;

        public NlogFormatGenerator(ILogger<NlogFormatGenerator> logger,
                                   QueuePublisher publisher,
                                   LogFileReader reader)
        {
            _logger = logger;
            _publisher = publisher;
            _reader = reader;
        }

        public async void log_simulator()
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

                throw;
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
