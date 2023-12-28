using Ardalis.Specification.EntityFrameworkCore;

namespace Bazaar.Catalog.Infrastructure.Repositories;

public class Repository<T> : RepositoryBase<T> where T : class
{
    public Repository(CatalogDbContext context) : base(context)
    {
    }
}