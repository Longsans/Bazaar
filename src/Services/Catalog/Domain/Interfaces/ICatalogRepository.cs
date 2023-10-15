namespace Bazaar.Catalog.Domain.Interfaces;

public interface ICatalogRepository
{
    IQueryable<CatalogItem> GetItems();
    CatalogItem? GetById(int id);
    CatalogItem? GetByProductId(string productId);
    IQueryable<CatalogItem> GetByNameSubstring(string nameSubstring);
    IQueryable<CatalogItem> GetBySellerId(string sellerId);
    CatalogItem Create(CatalogItem item);
    void Update(CatalogItem item);
    bool Delete(int id);
}