using System.Text.Json;
using AppFunction.Models;
using AppFunction.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppFunction.Services;

public class LogMessageProcessor : ILogMessageProcessor
{
    private readonly ILogger<LogMessageProcessor> _logger;

    public LogMessageProcessor(ILogger<LogMessageProcessor> logger)
    {
        _logger = logger;
    }

    public bool TryBuildEntity(string messageText, out LogEntity? entity)
    {
        entity = null;

        if (string.IsNullOrWhiteSpace(messageText))
        {
            _logger.LogWarning("[ERROR] STEP 1: Empty message received");
            return false;
        }

        LogMessage? logMessage;

        try
        {
            logMessage = JsonSerializer.Deserialize<LogMessage>(
                messageText,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "[ERROR] STEP 2: JSON deserialization failed");
            return false;
        }

        if (logMessage is null)
        {
            _logger.LogError("[ERROR] STEP 2: Deserialization returned null");
            return false;
        }

        if (logMessage.Event is null)
        {
            _logger.LogError("[ERROR] STEP 3: LogMessage.Event is null");
            return false;
        }

        if (string.IsNullOrWhiteSpace(logMessage.Source))
        {
            _logger.LogError("[ERROR] STEP 3: Source is null or empty");
            return false;
        }

        if (!DateTime.TryParse(logMessage.Event.Timestamp, out var parsedTimestamp))
        {
            _logger.LogError("[ERROR] STEP 4: Failed to parse timestamp: {Timestamp}", logMessage.Event.Timestamp);
            return false;
        }

        entity = new LogEntity
        {
            PartitionKey = logMessage.Source,
            RowKey = Guid.NewGuid().ToString(),
            Source = logMessage.Source,
            PublishedAtUtc = logMessage.PublishedAtUtc.ToUniversalTime(),
            EventTimestamp = parsedTimestamp.ToUniversalTime(),
            Level = string.IsNullOrWhiteSpace(logMessage.Event.Level) ? "UNKNOWN" : logMessage.Event.Level,
            Logger = string.IsNullOrWhiteSpace(logMessage.Event.Logger) ? "UNKNOWN" : logMessage.Event.Logger,
            Message = logMessage.Event.Message ?? string.Empty,
            Exception = logMessage.Event.Exception
        };

        return true;
    }
}
