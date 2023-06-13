using Bazaar.Catalog.Model;
namespace Bazaar.Catalog.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly List<CatalogItem> _items;
    private int _nextId => _items.Count + 1;
    private const string CATALOG_SECTION = "products";

    public CatalogRepository(JsonDataAdapter adapter)
    {
        _items = adapter.ReadToObjects<CatalogItem>(CATALOG_SECTION, (item, id) => item.Id = id).ToList();
    }

    public CatalogItem? GetItemById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public CatalogItem? GetItemByProductId(string productId)
    {
        return _items.FirstOrDefault(item => item.ProductId == productId);
    }

    public CatalogItem Create(CatalogItem item)
    {
        item.Id = _nextId;
        _items.Add(item);
        return item;
    }

    public void Update(CatalogItem item)
    {
        var existing = _items.FirstOrDefault(i => i.Id == item.Id);
        if (existing == null)
            return;
        _items.Remove(existing);
        _items.Add(item);
    }

    public void Delete(int id)
    {
        var existing = _items.FirstOrDefault(i => i.Id == id);
        if (existing == null)
            return;
        _items.Remove(existing);
    }
}