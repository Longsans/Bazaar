namespace Bazaar.Catalog.Model;

public interface ICatalogRepository
{
    CatalogItem? GetItemById(int id);
    CatalogItem Create(CatalogItem item);
    void Update(CatalogItem item);
    void Delete(int id);
}