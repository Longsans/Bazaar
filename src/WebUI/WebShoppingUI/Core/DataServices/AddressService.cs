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
    public string Catalog => $"{CATALOG_API}/api/catalog";
    public string CatalogByNameSubstring(string partOfName)
        => $"{CATALOG_API}/api/catalog?partOfName={partOfName}";
    public string CatalogByProductIds(string productIds)
        => $"{CATALOG_API}/api/catalog?productId={productIds}";

    // Ordering
    public string OrdersByBuyerId(string buyerId)
        => $"{ORDERING_API}/api/orders?buyerId={buyerId}";
    public string OrderById(int orderId)
        => $"{ORDERING_API}/api/orders/{orderId}";
    public string OrderStatusById(int orderId, OrderStatus status)
        => $"{ORDERING_API}/api/orders/{orderId}?status={status}";

    // Basket
    public string BasketByBuyerId(string buyerId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}";

    public string BasketItemsByBuyerId(string buyerId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}/items";

    public string BasketItemByBuyerAndProductId(string buyerId, string productId)
        => $"{BASKET_API}/api/buyer-baskets/{buyerId}/items/{productId}";

    public string BasketCheckout => $"{BASKET_API}/api/buyer-baskets/checkout";

    // Shopper info
    public string ShopperByExternalId(string externalId)
        => $"{SHOPPER_INFO_API}/api/shoppers?externalId={externalId}";
}
