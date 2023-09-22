namespace Bazaar.Catalog.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public CatalogRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public IQueryable<CatalogItem> GetItems()
    {
        return _context.CatalogItems;
    }

    public CatalogItem? GetById(int id)
    {
        return _context.CatalogItems.Find(id);
    }

    public CatalogItem? GetByProductId(string productId)
    {
        return _context.CatalogItems.FirstOrDefault(item => item.ProductId == productId);
    }

    public IQueryable<CatalogItem> GetByNameSubstring(string nameSubstring)
    {
        return _context.CatalogItems.Where(item => item.Name.Contains(nameSubstring));
    }

    public IQueryable<CatalogItem> GetBySellerId(string sellerId)
    {
        return _context.CatalogItems.Where(item => item.SellerId == sellerId);
    }

    public CatalogItem Create(CatalogItem item)
    {
        _context.CatalogItems.Add(item);
        _context.SaveChanges();
        return item;
    }

    public bool Update(CatalogItem update)
    {
        var existing = _context.CatalogItems.SingleOrDefault(item => item.ProductId == update.ProductId);

        if (existing == null)
            return false;

        existing.Name = update.Name;
        existing.Price = update.Price;
        existing.Description = update.Description;
        existing.AvailableStock = update.AvailableStock;
        existing.RestockThreshold = update.RestockThreshold;
        existing.MaxStockThreshold = update.MaxStockThreshold;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var existing = _context.CatalogItems.Find(id);
        if (existing == null)
            return false;
        _context.CatalogItems.Remove(existing);
        return true;
    }
}