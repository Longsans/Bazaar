namespace Bazaar.Transport.Infrastructure.Repositories;

public class InventoryReturnRepository : IInventoryReturnRepository
{
    private readonly TransportDbContext _context;

    public InventoryReturnRepository(TransportDbContext context)
    {
        _context = context;
    }

    public InventoryReturn? GetById(int id)
    {
        return _context.InventoryReturns.Include(x => x.ReturnQuantities)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<InventoryReturn> GetByInventoryOwnerId(string ownerId)
    {
        return _context.InventoryReturns.Include(x => x.ReturnQuantities)
            .Where(x => x.InventoryOwnerId == ownerId);
    }

    public IEnumerable<InventoryReturn> GetIncomplete()
    {
        return _context.InventoryReturns
            .Include(x => x.ReturnQuantities)
            .Where(x => x.Status == DeliveryStatus.Scheduled
                || x.Status == DeliveryStatus.Delivering);
    }

    public InventoryReturn Create(InventoryReturn inventoryReturn)
    {
        _context.InventoryReturns.Add(inventoryReturn);
        _context.SaveChanges();
        return inventoryReturn;
    }

    public void Update(InventoryReturn inventoryReturn)
    {
        _context.InventoryReturns.Update(inventoryReturn);
        _context.SaveChanges();
    }
}
