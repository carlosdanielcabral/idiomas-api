namespace Idiomas.Core.Interface.Service;

public interface IUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);

    Task ExecuteAsync(Func<Task> operation);
}
