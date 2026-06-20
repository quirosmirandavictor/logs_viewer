using Azure;
using Azure.Data.Tables;

namespace AppFunction.Models;

public class LogEntity : ITableEntity
{
    // ITableEntity
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // Mensaje original
    public string Source { get; set; } = default!;
    public DateTime PublishedAtUtc { get; set; }

    // Event
    public DateTime EventTimestamp { get; set; }
    public string Level { get; set; } = default!;
    public string Logger { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Exception { get; set; }
}