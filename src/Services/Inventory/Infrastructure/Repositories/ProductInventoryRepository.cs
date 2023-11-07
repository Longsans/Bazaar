namespace Bazaar.Inventory.Infrastructure.Repositories;

public class ProductInventoryRepository : IProductInventoryRepository
{
    private readonly InventoryDbContext _context;

    public ProductInventoryRepository(InventoryDbContext context)
    {
        _context = context;
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
}
