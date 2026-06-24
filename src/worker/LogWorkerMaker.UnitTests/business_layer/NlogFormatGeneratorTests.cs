using LogWorkerMaker.business_layer;
using LogWorkerMaker.infrastructure;
using LogWorkerMaker.infrastructure.Interfaces;
using LogWorkerMaker.models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LogWorkerMaker.UnitTests.business_layer;

public class NlogFormatGeneratorTests
{
    [Fact]
    public async Task LogSimulatorAsync_WhenBothSourcesHaveEvents_PublishesTwoMessages()
    {
        var publisher = new Mock<IQueuePublisher>();
        var reader = new Mock<ILogFileReader>();

        reader
            .Setup(x => x.GetLastEvent(It.Is<string>(file => file.StartsWith("worker-"))))
            .Returns(new LogEvent { Message = "nlog-message", Timestamp = "2026-06-23T10:00:00Z" });

        reader
            .Setup(x => x.GetLastEvent(It.Is<string>(file => file.StartsWith("python_format-"))))
            .Returns(new LogEvent { Message = "python-message", Timestamp = "2026-06-23T10:00:00Z" });

        var sut = new NlogFormatGenerator(NullLogger<NlogFormatGenerator>.Instance, publisher.Object, reader.Object);

        await sut.LogSimulatorAsync();

        publisher.Verify(x => x.PublishAsync(It.Is<LogQueueMessage>(m => m.Source == "nlog")), Times.Once);
        publisher.Verify(x => x.PublishAsync(It.Is<LogQueueMessage>(m => m.Source == "python")), Times.Once);
    }

    [Fact]
    public async Task LogSimulatorAsync_WhenOneSourceIsMissing_SkipsMissingSource()
    {
        var publisher = new Mock<IQueuePublisher>();
        var reader = new Mock<ILogFileReader>();

        reader
            .Setup(x => x.GetLastEvent(It.Is<string>(file => file.StartsWith("worker-"))))
            .Returns(new LogEvent { Message = "nlog-message", Timestamp = "2026-06-23T10:00:00Z" });

        reader
            .Setup(x => x.GetLastEvent(It.Is<string>(file => file.StartsWith("python_format-"))))
            .Returns((LogEvent?)null);

        var sut = new NlogFormatGenerator(NullLogger<NlogFormatGenerator>.Instance, publisher.Object, reader.Object);

        await sut.LogSimulatorAsync();

        publisher.Verify(x => x.PublishAsync(It.Is<LogQueueMessage>(m => m.Source == "nlog")), Times.Once);
        publisher.Verify(x => x.PublishAsync(It.Is<LogQueueMessage>(m => m.Source == "python")), Times.Never);
    }
}
