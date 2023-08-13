namespace Bazaar.Catalog.Repositories;

public interface ITransactionManager
{
    ITransaction BeginTransaction();
}
