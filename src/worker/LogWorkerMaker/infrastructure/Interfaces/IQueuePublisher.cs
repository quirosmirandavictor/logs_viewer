using LogWorkerMaker.models;

namespace LogWorkerMaker.infrastructure.Interfaces;

public interface IQueuePublisher
{
    Task PublishAsync(LogQueueMessage message);
}
