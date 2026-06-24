using Azure.Storage.Queues;
using LogWorkerMaker.infrastructure.Interfaces;
using LogWorkerMaker.models;
using System.Text.Json;

namespace LogWorkerMaker.infrastructure
{
    public class QueuePublisher : IQueuePublisher
    {
        private readonly QueueClient _queueClient;

        public QueuePublisher(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("Storage")!;

            string queueName =
                configuration["QueueName"] ?? "logsqueue";

            _queueClient = new QueueClient(
                connectionString,
                queueName,
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });

            _queueClient.CreateIfNotExists();
        }

        public async Task PublishAsync(LogQueueMessage message)
        {
            string json =
                JsonSerializer.Serialize(message);

            await _queueClient.SendMessageAsync(json);
        }
    }
}
