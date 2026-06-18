using LogWorkerMaker.infrastructure;

namespace LogWorkerMaker.models
{


    public class LogQueueMessage
    {
        public string Source { get; set; } = string.Empty;   // nlog | python

        public LogEvent Event { get; set; } = new();

        public DateTime PublishedAtUtc { get; set; }
    }
}
