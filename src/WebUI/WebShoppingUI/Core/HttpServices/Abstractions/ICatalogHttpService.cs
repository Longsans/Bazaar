namespace WebShoppingUI.HttpServices;

using CatalogItemResult = ServiceCallResult<CatalogItem>;
using CatalogServiceResult = ServiceCallResult<IEnumerable<CatalogItem>>;

public interface ICatalogHttpService
{
    Task<CatalogServiceResult> GetByNameSubstring(string nameSubstring);
    Task<CatalogItemResult> GetByProductId(string productId);
}
