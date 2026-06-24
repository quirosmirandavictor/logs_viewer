namespace LogWorkerMaker.infrastructure.Interfaces;

public interface ILogFileReader
{
    LogEvent? GetLastEvent(string fileName);
}
