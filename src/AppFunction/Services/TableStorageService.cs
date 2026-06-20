using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using AppFunction.Models;

namespace AppFunction.Services;

public class TableStorageService : ITableStorageService
{
    private readonly TableClient _tableClient;
    private readonly ILogger<TableStorageService> _logger;

    public TableStorageService(TableServiceClient serviceClient, ILogger<TableStorageService> logger)
    {
        _logger = logger;

        _logger.LogInformation("Initializing TableStorageService...");
        _logger.LogInformation("Received ServiceClient: {ServiceClientType}", serviceClient?.GetType().Name ?? "NULL");

        if (serviceClient == null)
        {
            _logger.LogError("CRITICAL ERROR: TableServiceClient is NULL");
            throw new ArgumentNullException(nameof(serviceClient), "TableServiceClient cannot be null");
        }

        _tableClient = serviceClient.GetTableClient("Logs");
        _logger.LogInformation("TableClient created for table 'Logs'");

        // Do not create the table synchronously in the constructor.
        // The table will be created during the first call to SaveAsync.
    }

    public async Task SaveAsync(LogEntity entity)
    {
        try
        {
            _logger.LogInformation(
                "Saving entity to Table Storage. PartitionKey: {PartitionKey}, RowKey: {RowKey}",
                entity.PartitionKey,
                entity.RowKey);

            // Create the table if it does not exist.
            try
            {
                await _tableClient.CreateIfNotExistsAsync();
                _logger.LogInformation("Table 'Logs' verified or created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create table. Continuing with save operation");
            }

            await _tableClient.AddEntityAsync(entity);

            _logger.LogInformation("Entity saved successfully");
        }
        catch (Azure.RequestFailedException rfe)
        {
            _logger.LogError(
                rfe,
                "Azure Table Storage error. Status: {Status}. Message: {Message}",
                rfe.Status,
                rfe.Message);

            throw new Exception(
                $"Failed to save entity to Table Storage (Status: {rfe.Status})",
                rfe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving entity");

            throw new Exception(
                "Unexpected error while saving entity to Table Storage",
                ex);
        }
    }
}