namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class ProductInventoryRepository : IProductInventoryRepository
{
    private readonly FbbInventoryDbContext _context;

    public ProductInventoryRepository(FbbInventoryDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ProductInventory> GetAll()
    {
        return _context.ProductInventories
            .Include(x => x.FulfillableLots)
            .Include(x => x.UnfulfillableLots)
            .Include(x => x.SellerInventory)
            .AsEnumerable();
    }

    public ProductInventory? GetById(int id)
    {
        return _context.ProductInventories
            .Include(x => x.FulfillableLots)
            .Include(x => x.UnfulfillableLots)
            .Include(x => x.SellerInventory)
            .SingleOrDefault(x => x.Id == id);
    }

    public ProductInventory? GetByProductId(string productId)
    {
        return _context.ProductInventories
            .Include(x => x.UnfulfillableLots)
            .Include(x => x.FulfillableLots)
            .Include(x => x.SellerInventory)
            .SingleOrDefault(x => x.ProductId == productId);
    }

    public void Update(ProductInventory productInventory)
    {
        _context.ProductInventories.Update(productInventory);
        _context.SaveChanges();
    }

    public void UpdateRange(IEnumerable<ProductInventory> productInventories)
    {
        _context.ProductInventories.UpdateRange(productInventories);
        _context.SaveChanges();
    }

    public void Delete(ProductInventory productInventory)
    {
        var entry = _context.Entry(productInventory);
        entry.State = EntityState.Deleted;
        _context.SaveChanges();
    }

    public void DeleteRange(IEnumerable<ProductInventory> productInventories)
    {
        foreach (var inventory in productInventories)
        {
            var entry = _context.Entry(inventory);
            entry.State = EntityState.Deleted;
        }
        _context.SaveChanges();
    }
}
