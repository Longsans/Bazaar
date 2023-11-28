namespace Bazaar.Disposal.Infrastructure.Repositories;

public class DisposalOrderRepository(DisposalDbContext context) : IDisposalOrderRepository
{
    private readonly DisposalDbContext _context = context;

    public DisposalOrder Create(DisposalOrder disposalOrder)
    {
        _context.DisposalOrders.Add(disposalOrder);
        _context.SaveChanges();
        return disposalOrder;
    }

    public DisposalOrder? GetById(int id)
    {
        return _context.DisposalOrders
            .Include(x => x.DisposeQuantities)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<DisposalOrder> GetByInventoryOwnerId(string inventoryOwnerId)
    {
        return _context.DisposalOrders
            .Include(x => x.DisposeQuantities)
            .Where(x => x.DisposeQuantities.Any(q => q.InventoryOwnerId == inventoryOwnerId));
    }

    public void Update(DisposalOrder disposalOrder)
    {
        _context.DisposalOrders.Update(disposalOrder);
        _context.SaveChanges();
    }
}
