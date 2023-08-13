namespace Bazaar.Catalog.Repositories;

public class EfCoreTransactionManager : ITransactionManager
{
    private readonly CatalogDbContext _context;

    public EfCoreTransactionManager(CatalogDbContext context)
    {
        _context = context;
    }

    public ITransaction BeginTransaction()
    {
        return new EfCoreTransaction(_context.Database.BeginTransaction());
    }
}
