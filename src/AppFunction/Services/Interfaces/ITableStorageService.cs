using AppFunction.Models;

namespace AppFunction.Services;

public interface ITableStorageService
{
    Task SaveAsync(LogEntity entity);
}