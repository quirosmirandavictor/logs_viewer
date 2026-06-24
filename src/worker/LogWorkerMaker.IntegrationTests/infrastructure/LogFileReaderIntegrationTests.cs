using LogWorkerMaker.infrastructure;
using Xunit;

namespace LogWorkerMaker.IntegrationTests.infrastructure;

public class LogFileReaderIntegrationTests
{
    [Fact]
    public void GetLastEvent_WhenFileHasMultipleRows_ReturnsLastEvent()
    {
        var logsDirectory = Path.Combine(Path.GetTempPath(), "logs-viewer-tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(logsDirectory);

        try
        {
            var filePath = Path.Combine(logsDirectory, "worker-2026-06-23.json");
            File.WriteAllLines(filePath,
            [
                "{\"timestamp\":\"2026-06-23 10:00:00\",\"level\":\"Info\",\"logger\":\"A\",\"message\":\"first\"}",
                "{\"timestamp\":\"2026-06-23 10:01:00\",\"level\":\"Error\",\"logger\":\"B\",\"message\":\"second\"}"
            ]);

            var reader = new LogFileReader(logsDirectory);
            var last = reader.GetLastEvent("worker-2026-06-23.json");

            Assert.NotNull(last);
            Assert.Equal("Error", last!.Level);
            Assert.Equal("second", last.Message);
        }
        finally
        {
            if (Directory.Exists(logsDirectory))
            {
                Directory.Delete(logsDirectory, recursive: true);
            }
        }
    }
}
