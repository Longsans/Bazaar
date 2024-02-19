using Flurl;

namespace WebShoppingUI.DataServices;

public class ApiEndpointResolver
{
    private readonly string CATALOG_API;
    private readonly string ORDERING_API;
    private readonly string SHOPPER_INFO_API;
    private readonly string BASKET_API;
    private readonly string CONTRACTING_API;

    public ApiEndpointResolver(IConfiguration config)
    {
        CATALOG_API = config["CatalogApi"]!;
        ORDERING_API = config["OrderingApi"]!;
        SHOPPER_INFO_API = config["ShopperInfoApi"]!;
        BASKET_API = config["BasketApi"]!;
        CONTRACTING_API = config["ContractingApi"]!;
    }

    // Catalog
    public string CatalogByNameSubstring(string nameSubstring)
        => Url.Combine(CATALOG_API, $"/api/catalog?nameSubstring={nameSubstring}");

    public string CatalogItemByProductId(string productId)
        => Url.Combine(CATALOG_API, $"/api/catalog/{productId}");

    // Ordering
    public string OrdersByBuyerId(string buyerId)
        => Url.Combine(ORDERING_API, $"/api/orders?buyerId={buyerId}");

    public string OrderById(int orderId)
        => Url.Combine(ORDERING_API, $"/api/orders/{orderId}");

    public string OrdersByBuyerIdAndProductIds(string buyerId, string[] productIds)
        => $"{OrdersByBuyerId(buyerId)}&productId={string.Join(',', productIds)}";

    // Basket
    public string BasketByBuyerId(string buyerId)
        => Url.Combine(BASKET_API, $"/api/buyer-baskets/{buyerId}");

    public string BasketItemsByBuyerId(string buyerId)
        => Url.Combine(BASKET_API, $"/api/buyer-baskets/{buyerId}/items");

    public string BasketItemByBuyerAndProductId(string buyerId, string productId)
        => Url.Combine(BASKET_API, $"/api/buyer-baskets/{buyerId}/items/{productId}");

    public string BasketCheckouts => Url.Combine(BASKET_API, "/api/checkouts");

    // Shopper info
    public string Shoppers => Url.Combine(SHOPPER_INFO_API, "/api/shoppers");
    public string ShopperByExternalIdQuery(string externalId) => Url.Combine(SHOPPER_INFO_API, $"/api/shoppers?externalId={externalId}");
    public string ShopperPersonalInfo(string externalId) => Url.Combine(SHOPPER_INFO_API, $"/api/shoppers/{externalId}/personal-info");
    public string ShopperEmailAddress(string externalId) => Url.Combine(SHOPPER_INFO_API, $"/api/shoppers/{externalId}/email-address");

    // Contracting
    public string SellerById(string sellerId) => Url.Combine(CONTRACTING_API, $"/api/clients?externalId={sellerId}");
}