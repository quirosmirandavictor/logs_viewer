using Azure.Storage.Queues.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using AppFunction.Models;
using AppFunction.Services;

namespace AppFunction;

public class LogsQueueFunction
{
    private readonly ILogger<LogsQueueFunction> _logger;
    private readonly ITableStorageService _storage;

    public LogsQueueFunction(ILogger<LogsQueueFunction> logger,ITableStorageService storage)
    {
        _logger = logger;
        _storage = storage;
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

            // Step 2: JSON deserialization
            _logger.LogInformation("STEP 2: Attempting to deserialize JSON...");
            LogMessage logMessage;
            try
            {
                logMessage = JsonSerializer.Deserialize<LogMessage>(
                    message.MessageText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                _logger.LogInformation("[OK] STEP 2: JSON deserialized successfully");
            }
            catch (JsonException jex)
            {
                _logger.LogError(
                    jex,
                    "[ERROR] STEP 2: JSON deserialization failed. MessageText: {MessageText}",
                    message.MessageText);

                return;
            }

            if (logMessage == null)
            {
                _logger.LogError(
                    "[ERROR] STEP 2: Deserialization returned null. MessageText: {MessageText}",
                    message.MessageText);

                return;
            }

            _logger.LogInformation(
                "[OK] STEP 2: LogMessage is not null. Source: {Source}",
                logMessage.Source);

            // Step 3: Required field validation
            _logger.LogInformation("STEP 3: Validating required fields...");

            if (logMessage.Event == null)
            {
                _logger.LogError("[ERROR] STEP 3: LogMessage.Event is null");
                return;
            }

            _logger.LogInformation("[OK] STEP 3: Event is valid");

            if (string.IsNullOrEmpty(logMessage.Source))
            {
                _logger.LogError("[ERROR] STEP 3: Source is null or empty");
                return;
            }

            _logger.LogInformation(
                "[OK] STEP 3: Source is valid: {Source}",
                logMessage.Source);

            // Step 4: Map to LogEntity
            _logger.LogInformation("STEP 4: Mapping to LogEntity...");

            DateTime parsedTimestamp;

            try
            {
                _logger.LogInformation(
                    "Parsing timestamp: {Timestamp}",
                    logMessage.Event.Timestamp);

                parsedTimestamp = DateTime.Parse(logMessage.Event.Timestamp);

                _logger.LogInformation("[OK] STEP 4: Timestamp parsed successfully");
            }
            catch (FormatException fex)
            {
                _logger.LogError(
                    fex,
                    "[ERROR] STEP 4: Failed to parse timestamp: {Timestamp}",
                    logMessage.Event.Timestamp);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[ERROR] STEP 4: Unexpected error while parsing timestamp");

                return;
            }

            var entity = new LogEntity
            {
                PartitionKey = logMessage.Source,
                RowKey = Guid.NewGuid().ToString(),

                Source = logMessage.Source,
                PublishedAtUtc = logMessage.PublishedAtUtc.ToUniversalTime(),

                EventTimestamp = parsedTimestamp.ToUniversalTime(),
                Level = logMessage.Event.Level ?? "UNKNOWN",
                Logger = logMessage.Event.Logger ?? "UNKNOWN",
                Message = logMessage.Event.Message ?? string.Empty,
                Exception = logMessage.Event.Exception
            };

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