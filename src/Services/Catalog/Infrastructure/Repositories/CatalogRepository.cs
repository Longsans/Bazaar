namespace Bazaar.Catalog.Infrastructure.Repositories;

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
        return _context.CatalogItems.Where(item => item.ProductName.Contains(nameSubstring));
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

    public void Update(CatalogItem update)
    {
        _context.CatalogItems.Update(update);
        _context.SaveChanges();
    }

    public void UpdateRange(IEnumerable<CatalogItem> items)
    {
        _context.CatalogItems.UpdateRange(items);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var existing = _context.CatalogItems.Find(id);
        if (existing == null)
            return;

        _context.CatalogItems.Remove(existing);
        _context.SaveChanges();
    }
}