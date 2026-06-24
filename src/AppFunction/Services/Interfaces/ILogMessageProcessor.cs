using AppFunction.Models;

namespace AppFunction.Services.Interfaces;

public interface ILogMessageProcessor
{
    bool TryBuildEntity(string messageText, out LogEntity? entity);
}
