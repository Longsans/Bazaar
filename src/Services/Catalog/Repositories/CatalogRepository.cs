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

    public CatalogItem? GetItemById(int id)
    {
        return _context.CatalogItems.Find(id);
    }

    public CatalogItem? GetItemByProductId(string productId)
    {
        return _context.CatalogItems.FirstOrDefault(item => item.ProductId == productId);
    }

    public IQueryable<CatalogItem> GetManyByProductId(IEnumerable<string> productIds)
    {
        return _context.CatalogItems.Where(item => productIds.Contains(item.ProductId));
    }

    public CatalogItem Create(CatalogItem item)
    {
        _context.CatalogItems.Add(item);
        _context.SaveChanges();
        return item;
    }

    public bool Update(CatalogItem item)
    {
        var existing = _context.CatalogItems.Find(item.Id);
        if (existing == null)
            return false;
        _context.CatalogItems.Entry(existing).CurrentValues.SetValues(item);
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