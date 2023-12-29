using Ardalis.Specification.EntityFrameworkCore;

namespace Bazaar.Contracting.Infrastructure.Repositories;

public class Repository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public Repository(ContractingDbContext context) : base(context)
    {
    }
}
