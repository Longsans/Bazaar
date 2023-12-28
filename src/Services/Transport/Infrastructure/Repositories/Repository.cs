﻿using Ardalis.Specification.EntityFrameworkCore;

namespace Bazaar.Transport.Infrastructure.Repositories;

public class Repository<T> : RepositoryBase<T> where T : class
{
    public Repository(TransportDbContext context) : base(context) { }
}
