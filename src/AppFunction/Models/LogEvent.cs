
namespace AppFunction.Models;

public class LogEvent
{
    public string Timestamp { get; set; } = default!;
    public string Level { get; set; } = default!;
    public string Logger { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Exception { get; set; }
}