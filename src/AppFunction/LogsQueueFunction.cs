using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AppFunction.Services;
using AppFunction.Services.Interfaces;

namespace AppFunction;

public class LogsQueueFunction
{
    private readonly ILogger<LogsQueueFunction> _logger;
    private readonly ITableStorageService _storage;
    private readonly ILogMessageProcessor _messageProcessor;

    public LogsQueueFunction(
        ILogger<LogsQueueFunction> logger,
        ITableStorageService storage,
        ILogMessageProcessor messageProcessor)
    {
        _logger = logger;
        _storage = storage;
        _messageProcessor = messageProcessor;
    }

    /*[Function(nameof(LogsQueueFunction))]
    public Task Run(
        [QueueTrigger("logsqueue")] string message)
    {
        _logger.LogInformation(message);

        return Task.CompletedTask;
    }*/
    
    [Function(nameof(LogsQueueFunction))]
    public async Task Run([QueueTrigger("logsqueue")] QueueMessage message)
    {
        try
        {
            _logger.LogInformation("Starting message processing");
            _logger.LogInformation("MessageId: {MessageId}", message.MessageId);
            _logger.LogInformation("RawMessage: {MessageText}", message.MessageText);

            // Step 1: Basic validation
            if (message == null)
            {
                _logger.LogError("[ERROR] STEP 1: Message is null");
                return;
            }

            if (string.IsNullOrEmpty(message.MessageText))
            {
                _logger.LogWarning("[ERROR] STEP 1: Empty message received");
                return;
            }

            _logger.LogInformation("[OK] STEP 1: Basic validation completed");

            if (!_messageProcessor.TryBuildEntity(message.MessageText, out var entity) || entity is null)
            {
                return;
            }

            _logger.LogInformation(
                "[OK] STEP 4: LogEntity created. PartitionKey: {PartitionKey}, RowKey: {RowKey}",
                entity.PartitionKey,
                entity.RowKey);

            // Step 5: Save to Table Storage
            _logger.LogInformation("STEP 5: Saving entity to Table Storage...");

            try
            {
                await _storage.SaveAsync(entity);

                _logger.LogInformation(
                    "[OK] STEP 5: Entity saved successfully. RowKey: {RowKey}",
                    entity.RowKey);
            }
            catch (Exception storageEx)
            {
                _logger.LogError(
                    storageEx,
                    "[ERROR] STEP 5: Failed to save entity to Table Storage. Entity: {@Entity}",
                    entity);

                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[ERROR] Unhandled exception in LogsQueueFunction");

            throw;
        }
    }
    
}