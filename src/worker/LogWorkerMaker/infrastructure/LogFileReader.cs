using System.Text.Json;
using System.Text.Json.Serialization;
using LogWorkerMaker.infrastructure.Interfaces;

namespace LogWorkerMaker.infrastructure
{
    public class LogFileReader : ILogFileReader
    {
        private readonly string _logsDirectory;

        public LogFileReader()
            : this(Path.Combine(AppContext.BaseDirectory, "logs"))
        {
        }

        public LogFileReader(string logsDirectory)
        {
            _logsDirectory = logsDirectory;
        }

        public LogEvent? GetLastEvent(string fileName)
        {
            string path = Path.Combine(_logsDirectory, fileName);

            if (!File.Exists(path))
                return null;

            using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            using var reader = new StreamReader(stream);

            string? line;
            string? last = null;

            while ((line = reader.ReadLine()) != null)
            {
                last = line;
            }

            if (last == null)
                return null;

            return JsonSerializer.Deserialize<LogEvent>(last);
        }
    }

    public class LogEvent
    {

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonPropertyName("level")]
        public string Level { get; set; } = string.Empty;

        [JsonPropertyName("logger")]
        public string Logger { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("exception")]
        public string? Exception { get; set; }
    }
}