using AppFunction.Models;

namespace AppFunction.Services;

public interface ITableStorageService
{
    Task<IReadOnlyList<LogEntity>> GetAllAsync();
    Task SaveAsync(LogEntity entity);
}