using AppFunction.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace AppFunction.UnitTests.Services;

public class LogMessageProcessorTests
{
    private readonly LogMessageProcessor _processor = new(NullLogger<LogMessageProcessor>.Instance);

    [Fact]
    public void TryBuildEntity_WhenJsonIsValid_ReturnsEntity()
    {
        var json = """
                   {
                     "source": "nlog",
                     "publishedAtUtc": "2026-06-23T10:15:00Z",
                     "event": {
                       "timestamp": "2026-06-23T10:14:59Z",
                       "level": "Error",
                       "logger": "Worker",
                       "message": "Boom",
                       "exception": "System.Exception"
                     }
                   }
                   """;

        var success = _processor.TryBuildEntity(json, out var entity);

        Assert.True(success);
        Assert.NotNull(entity);
        Assert.Equal("nlog", entity!.PartitionKey);
        Assert.Equal("Error", entity.Level);
        Assert.Equal("Worker", entity.Logger);
        Assert.Equal("Boom", entity.Message);
    }

    [Fact]
    public void TryBuildEntity_WhenSourceIsMissing_ReturnsFalse()
    {
        var json = """
                   {
                     "publishedAtUtc": "2026-06-23T10:15:00Z",
                     "event": {
                       "timestamp": "2026-06-23T10:14:59Z",
                       "level": "Error",
                       "logger": "Worker",
                       "message": "Boom"
                     }
                   }
                   """;

        var success = _processor.TryBuildEntity(json, out var entity);

        Assert.False(success);
        Assert.Null(entity);
    }

    [Fact]
    public void TryBuildEntity_WhenTimestampIsInvalid_ReturnsFalse()
    {
        var json = """
                   {
                     "source": "python",
                     "publishedAtUtc": "2026-06-23T10:15:00Z",
                     "event": {
                       "timestamp": "not-a-date",
                       "level": "Error",
                       "logger": "Worker",
                       "message": "Boom"
                     }
                   }
                   """;

        var success = _processor.TryBuildEntity(json, out var entity);

        Assert.False(success);
        Assert.Null(entity);
    }
}
