namespace WebShoppingUI.DataServices;

using CatalogItemResult = ServiceCallResult<CatalogItem>;
using CatalogServiceResult = ServiceCallResult<IEnumerable<CatalogItem>>;

public interface ICatalogDataService
{
    Task<CatalogServiceResult> GetByNameSubstring(string nameSubstring);
    Task<CatalogItemResult> GetByProductId(string productId);
}
