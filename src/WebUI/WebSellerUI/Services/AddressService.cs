namespace WebSellerUI.Services;

public class AddressService
{
    private readonly string CATALOG_API;
    private readonly string ORDERING_API;
    private readonly string CONTRACTING_API;

    public AddressService(IConfiguration config)
    {
        CATALOG_API = config["CatalogApi"]!;
        ORDERING_API = config["OrderingApi"]!;
        CONTRACTING_API = config["ContractingApi"]!;
    }

    // Catalog
    public string CatalogBySellerId(string sellerId) =>
        $"{CATALOG_API}/api/catalog?sellerId={sellerId}";

    public string Catalog => $"{CATALOG_API}/api/catalog";

    // Ordering
    public string OrdersWithProductIds(string productIds) =>
        $"{ORDERING_API}/api/orders?productId={productIds}";

    // Contracting
    public string PartnerById(string partnerId) =>
        $"{CONTRACTING_API}/api/partners?externalId={partnerId}";

    public string ContractsByPartnerId(string partnerId) =>
        $"{CONTRACTING_API}/api/contracts?partnerId={partnerId}";

}
