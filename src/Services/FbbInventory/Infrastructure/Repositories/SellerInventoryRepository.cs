using Microsoft.EntityFrameworkCore;

namespace Bazaar.FbbInventory.Infrastructure.Repositories;

public class SellerInventoryRepository : ISellerInventoryRepository
{
    private readonly InventoryDbContext _context;

    public SellerInventoryRepository(InventoryDbContext context)
    {
        _context = context;
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
