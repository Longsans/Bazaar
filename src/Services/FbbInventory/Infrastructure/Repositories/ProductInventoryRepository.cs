namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class ProductInventoryRepository : IProductInventoryRepository
{
    private readonly InventoryDbContext _context;

    public ProductInventoryRepository(InventoryDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ProductInventory> GetAll()
    {
        return _context.ProductInventories.AsEnumerable();
    }

    public ProductInventory? GetById(int id)
    {
        return _context.ProductInventories
            .SingleOrDefault(x => x.Id == id);
    }

    public ProductInventory? GetByProductId(string productId)
    {
        return _context.ProductInventories
            .SingleOrDefault(x => x.ProductId == productId);
    }

    public void Update(ProductInventory productInventory)
    {
        _context.ProductInventories.Update(productInventory);
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
