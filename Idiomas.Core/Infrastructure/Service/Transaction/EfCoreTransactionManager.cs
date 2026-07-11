using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Interface.Service;
using Microsoft.EntityFrameworkCore.Storage;

namespace Idiomas.Core.Infrastructure.Service.Transaction;

public class EfCoreTransactionManager(ApplicationContext context) : ITransactionManager
{
    private readonly ApplicationContext _context = context;

    public async Task<IDatabaseTransaction> BeginTransactionAsync()
    {
        IDbContextTransaction transaction = await this._context.Database.BeginTransactionAsync();

        return new EfCoreDatabaseTransaction(transaction);
    }
}

internal class EfCoreDatabaseTransaction(IDbContextTransaction transaction) : IDatabaseTransaction
{
    private readonly IDbContextTransaction _transaction = transaction;

    public async Task CommitAsync()
    {
        await this._transaction.CommitAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await this._transaction.DisposeAsync();
    }
}
