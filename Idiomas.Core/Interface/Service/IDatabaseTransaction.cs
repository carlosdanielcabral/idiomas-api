namespace Idiomas.Core.Interface.Service;

public interface IDatabaseTransaction : IAsyncDisposable
{
    Task CommitAsync();
}
