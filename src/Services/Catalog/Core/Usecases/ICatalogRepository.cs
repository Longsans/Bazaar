namespace Bazaar.Catalog.Core.Usecases;

public interface ICatalogRepository
{
    CatalogItem? GetItemById(int id);
    CatalogItem? GetItemByProductId(string externalId);
    CatalogItem Create(CatalogItem item);
    void Update(CatalogItem item);
    void Delete(int id);
}