namespace WebShoppingUI.DataServices;

using CatalogItemResult = ServiceCallResult<CatalogItem>;
using CatalogResult = ServiceCallResult<IEnumerable<CatalogItem>>;

public interface ICatalogDataService
{
    Task<CatalogResult> GetByNameSubstring(string nameSubstring);
    Task<CatalogItemResult> GetByProductId(string productId);
}
