namespace Bazaar.Transport.Infrastructure.Repositories;

public class InventoryPickupRepository : IInventoryPickupRepository
{
    private readonly TransportDbContext _context;

    public InventoryPickupRepository(TransportDbContext context)
    {
        _context = context;
    }

    public InventoryPickup? GetById(int id)
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<InventoryPickup> GetByProductId(string productId)
    {
        return _context.InventoryPickups.Where(
            x => x.ProductInventories.Any(i => i.ProductId == productId));
    }

    public IEnumerable<InventoryPickup> GetIncomplete()
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.Status != InventoryPickupStatus.Completed
                && x.Status != InventoryPickupStatus.Cancelled);
    }

    public IEnumerable<InventoryPickup> GetScheduled(string productId)
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.ProductInventories.Any(i => i.ProductId == productId)
                && x.Status == InventoryPickupStatus.Scheduled);
    }

    public IEnumerable<InventoryPickup> GetInProgress(string productId)
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.ProductInventories.Any(i => i.ProductId == productId)
                && x.Status == InventoryPickupStatus.EnRouteToPickupLocation
                || x.Status == InventoryPickupStatus.DeliveringToWarehouse);
    }

    public IEnumerable<InventoryPickup> GetCompleted(string productId)
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.ProductInventories.Any(i => i.ProductId == productId)
                && x.Status == InventoryPickupStatus.Completed);
    }

    public IEnumerable<InventoryPickup> GetCancelled(string productId)
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.ProductInventories.Any(i => i.ProductId == productId)
                && x.Status == InventoryPickupStatus.Cancelled);
    }

    public IEnumerable<InventoryPickup> GetAllCancelled()
    {
        return _context.InventoryPickups
            .Include(x => x.ProductInventories)
            .Where(x => x.Status == InventoryPickupStatus.Cancelled);
    }

    public InventoryPickup Create(InventoryPickup inventoryPickup)
    {
        _context.InventoryPickups.Add(inventoryPickup);
        _context.SaveChanges();
        return inventoryPickup;
    }

    public void Update(InventoryPickup inventoryPickup)
    {
        _context.InventoryPickups.Update(inventoryPickup);
        _context.SaveChanges();
    }

    public void Delete(InventoryPickup inventoryPickup)
    {
        _context.InventoryPickups.Remove(inventoryPickup);
        _context.SaveChanges();
    }
}
