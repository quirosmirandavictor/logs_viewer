using Azure.Storage.Queues;
using LogWorkerMaker.infrastructure;
using LogWorkerMaker.models;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LogWorkerMaker.IntegrationTests.infrastructure;

public class QueuePublisherIntegrationTests
{
    private const string AzuriteConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    [Fact]
    public async Task PublishAsync_WhenAzuriteIsAvailable_EnqueuesMessage()
    {
        var queueName = $"logsqueueint{Guid.NewGuid():N}";

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Storage"] = AzuriteConnectionString,
                ["QueueName"] = queueName
            })
            .Build();

        var publisher = new QueuePublisher(configuration);

        await publisher.PublishAsync(new LogQueueMessage
        {
            Source = "integration-worker",
            PublishedAtUtc = DateTime.UtcNow,
            Event = new LogWorkerMaker.infrastructure.LogEvent
            {
                Timestamp = DateTime.UtcNow.ToString("O"),
                Level = "Info",
                Logger = "QueuePublisherIntegration",
                Message = "integration-message"
            }
        });

        var queueClient = new QueueClient(
            AzuriteConnectionString,
            queueName,
            new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

        var response = await queueClient.ReceiveMessagesAsync(maxMessages: 1);

        Assert.Single(response.Value);
        Assert.Contains("integration-worker", response.Value[0].MessageText);
    }
}
