using Microsoft.EntityFrameworkCore.Storage;

namespace Bazaar.Catalog.Repositories;

public class EfCoreTransaction : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfCoreTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task Commit()
    {
        await _transaction.CommitAsync();
    }

    public async Task Rollback()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}
