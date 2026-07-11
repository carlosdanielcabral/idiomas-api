namespace Idiomas.Core.Interface.Service;

public interface ITransactionManager
{
    Task<IDatabaseTransaction> BeginTransactionAsync();
}
