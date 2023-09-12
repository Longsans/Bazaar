namespace WebShoppingUI.Services;

public class AddressService
{
    private readonly string CATALOG_API;
    private readonly string ORDERING_API;
    private readonly string SHOPPER_INFO_API;

    public AddressService(IConfiguration config)
    {
        CATALOG_API = config["CatalogApi"]!;
        ORDERING_API = config["OrderingApi"]!;
        SHOPPER_INFO_API = config["ShopperInfoApi"]!;
    }

    // Catalog
    public string Catalog => $"{CATALOG_API}/api/catalog";

    // Ordering
    public string OrdersByBuyerId(string buyerId) =>
        $"{ORDERING_API}/api/orders?buyerId={buyerId}";

    // Shopper info
    public string ShopperByExternalId(string externalId) =>
        $"{SHOPPER_INFO_API}/api/shoppers?externalId={externalId}";
}
