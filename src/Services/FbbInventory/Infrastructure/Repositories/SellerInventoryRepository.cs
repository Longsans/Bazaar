namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class SellerInventoryRepository : ISellerInventoryRepository
{
    private readonly FbbInventoryDbContext _context;

    public SellerInventoryRepository(FbbInventoryDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SellerInventory> GetAll()
    {
        return _context.SellerInventories;
    }

    public SellerInventory? GetWithProductsById(int id)
    {
        return _context.SellerInventories
            .Include(x => x.ProductInventories)
            .SingleOrDefault(x => x.Id == id);
    }

    public SellerInventory? GetWithProductsBySellerId(string sellerId)
    {
        return _context.SellerInventories
            .Include(x => x.ProductInventories)
            .SingleOrDefault(x => x.SellerId == sellerId);
    }

    public void Update(SellerInventory sellerInventory)
    {
        _context.SellerInventories.Update(sellerInventory);
        _context.SaveChanges();
    }
}
