namespace Bazaar.Catalog.Core.Usecases;

public interface ICatalogRepository
{
    IQueryable<CatalogItem> GetItems();
    CatalogItem? GetById(int id);
    CatalogItem? GetByProductId(string productId);
    IQueryable<CatalogItem> GetByNameSubstring(string nameSubstring);
    IQueryable<CatalogItem> GetBySellerId(string sellerId);
    CatalogItem Create(CatalogItem item);
    bool Update(CatalogItem item);
    bool Delete(int id);
}