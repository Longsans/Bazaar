namespace Bazaar.Catalog.Repositories;

public interface ITransaction : IDisposable
{
    Task Commit();
    Task Rollback();
}
