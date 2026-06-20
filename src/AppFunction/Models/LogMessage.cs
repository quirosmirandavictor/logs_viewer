
namespace AppFunction.Models;
public class LogMessage
{
    public string Source { get; set; } = default!;
    public LogEvent Event { get; set; } = default!;
    public DateTime PublishedAtUtc { get; set; }
}