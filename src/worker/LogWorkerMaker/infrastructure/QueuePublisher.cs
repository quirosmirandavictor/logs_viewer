using Azure.Storage.Queues;
using LogWorkerMaker.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace LogWorkerMaker.infrastructure
{
    public class QueuePublisher
    {
        private readonly QueueClient _queueClient;

        public QueuePublisher(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("Storage")!;

            _queueClient = new QueueClient(
                connectionString,
                "logsqueue",new QueueClientOptions
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
