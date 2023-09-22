namespace Bazaar.Catalog.DTOs;

public class CatalogItemUpdateCommand
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
