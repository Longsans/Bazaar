namespace WebShoppingUI.DataServices;

public class AddressService
{
    private readonly string CATALOG_API;
    private readonly string ORDERING_API;
    private readonly string SHOPPER_INFO_API;
    private readonly string BASKET_API;

    public AddressService(IConfiguration config)
    {
        CATALOG_API = config["CatalogApi"]!;
        ORDERING_API = config["OrderingApi"]!;
        SHOPPER_INFO_API = config["ShopperInfoApi"]!;
        BASKET_API = config["BasketApi"]!;
    }

    // Catalog
    public string CatalogByNameSubstring(string nameSubstring)
        => $"{CATALOG_API}/api/catalog?nameSubstring={nameSubstring}";
    public string CatalogItemByProductId(string productId)
        => $"{CATALOG_API}/api/catalog/{productId}";

    // Ordering
    public string OrdersByBuyerId(string buyerId)
        => $"{ORDERING_API}/api/orders?buyerId={buyerId}";
    public string OrderById(int orderId)
        => $"{ORDERING_API}/api/orders/{orderId}";
    public string OrdersByBuyerIdAndProductIds(string buyerId, string[] productIds)
        => $"{OrdersByBuyerId(buyerId)}&productId={string.Join(',', productIds)}";

    // Basket
    public string BasketByBuyerId(string buyerId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}";

    public string BasketItemsByBuyerId(string buyerId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}/items";

    public string BasketItemByBuyerAndProductId(string buyerId, string productId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}/items/{productId}";

    public string BasketCheckouts => $"{BASKET_API}/api/checkouts";

    // Shopper info
    public string ShopperByExternalIdQuery(string externalId)
        => $"{SHOPPER_INFO_API}/api/shoppers?externalId={externalId}";

    public string ShopperByExternalId(string externalId)
        => $"{SHOPPER_INFO_API}/api/shoppers/{externalId}";
    public string Shoppers
        => $"{SHOPPER_INFO_API}/api/shoppers";
}
