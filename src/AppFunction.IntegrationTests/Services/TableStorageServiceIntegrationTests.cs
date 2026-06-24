using AppFunction.Models;
using AppFunction.Services;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AppFunction.IntegrationTests.Services;

public class TableStorageServiceIntegrationTests
{
    private const string AzuriteConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    [Fact]
    public async Task SaveAsync_WhenAzuriteIsAvailable_PersistsEntity()
    {
        var serviceClient = new TableServiceClient(AzuriteConnectionString);
        var service = new TableStorageService(serviceClient, NullLogger<TableStorageService>.Instance);

        var entity = new LogEntity
        {
            PartitionKey = "integration-appfunction",
            RowKey = Guid.NewGuid().ToString(),
            Source = "nlog",
            PublishedAtUtc = DateTime.UtcNow,
            EventTimestamp = DateTime.UtcNow,
            Level = "Info",
            Logger = "Integration",
            Message = "Smoke test"
        };

        await service.SaveAsync(entity);

        var tableClient = serviceClient.GetTableClient("Logs");
        var saved = await tableClient.GetEntityAsync<LogEntity>(entity.PartitionKey, entity.RowKey);

        Assert.Equal(entity.RowKey, saved.Value.RowKey);
        Assert.Equal("nlog", saved.Value.Source);
    }
}
