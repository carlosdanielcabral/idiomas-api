using Idiomas.Core.Infrastructure.Database.Context;
using Idiomas.Core.Interface.Service;
using Microsoft.EntityFrameworkCore.Storage;

namespace Idiomas.Core.Infrastructure.Service.Transaction;

public class EfCoreUnitOfWork(ApplicationContext context) : IUnitOfWork
{
    private readonly ApplicationContext _context = context;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        await using IDbContextTransaction transaction = await this._context.Database.BeginTransactionAsync();

        T result = await operation();

        await transaction.CommitAsync();

        return result;
    }

    public async Task ExecuteAsync(Func<Task> operation)
    {
        await using IDbContextTransaction transaction = await this._context.Database.BeginTransactionAsync();

        await operation();

        await transaction.CommitAsync();
    }
}
