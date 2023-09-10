namespace Bazaar.Catalog.Core.Usecases;

public interface ICatalogRepository
{
    IQueryable<CatalogItem> GetItems();
    CatalogItem? GetItemById(int id);
    CatalogItem? GetItemByProductId(string externalId);
    IQueryable<CatalogItem> GetManyByProductId(IEnumerable<string> productIds);
    CatalogItem Create(CatalogItem item);
    bool Update(CatalogItem item);
    bool Delete(int id);
}