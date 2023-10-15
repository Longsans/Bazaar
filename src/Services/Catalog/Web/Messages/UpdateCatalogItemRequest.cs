namespace Bazaar.Catalog.Web.Messages;

public class UpdateCatalogItemRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }

    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }

    public CatalogItem ToCatalogItem(string productId) => new()
    {
        ProductId = productId,
        Name = Name,
        Description = Description,
        Price = Price,
        AvailableStock = AvailableStock,
        RestockThreshold = RestockThreshold,
        MaxStockThreshold = MaxStockThreshold
    };
}
