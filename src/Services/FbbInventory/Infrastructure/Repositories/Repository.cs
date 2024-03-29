﻿using Ardalis.Specification.EntityFrameworkCore;

namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class Repository<T> : RepositoryBase<T>, IRepository<T> where T : class
{
    public Repository(FbbInventoryDbContext context) : base(context)
    {
    }
}
