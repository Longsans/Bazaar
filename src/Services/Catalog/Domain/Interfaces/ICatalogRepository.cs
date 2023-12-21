namespace Bazaar.Catalog.Domain.Interfaces;

public interface ICatalogRepository
{
    CatalogItem? GetById(int id);
    CatalogItem? GetByProductId(string productId);
    IQueryable<CatalogItem> GetByNameSubstring(string nameSubstring);
    IQueryable<CatalogItem> GetBySellerId(string sellerId);
    CatalogItem Create(CatalogItem item);
    void Update(CatalogItem item);
    void UpdateRange(IEnumerable<CatalogItem> items);
    void Delete(int id);
}