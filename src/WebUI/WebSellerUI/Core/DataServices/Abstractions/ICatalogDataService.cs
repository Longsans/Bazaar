namespace WebSellerUI.DataServices;

using CatalogCollectionResult = ServiceCallResult<IEnumerable<CatalogItem>>;

public interface ICatalogDataService
{
    Task<CatalogCollectionResult> GetBySellerId(string sellerId);
    Task<ServiceCallResult<CatalogItem>> Create(CatalogItemCreateCommand item);
    Task<ServiceCallResult> Update(string productId, CatalogItemUpdateCommand update);
}
