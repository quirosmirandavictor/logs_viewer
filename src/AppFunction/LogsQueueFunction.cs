using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AppFunction;

public class LogsQueueFunction
{
    private readonly ILogger<LogsQueueFunction> _logger;

    public LogsQueueFunction(ILogger<LogsQueueFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(LogsQueueFunction))]
    public void Run([QueueTrigger("logsqueue")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    }
}