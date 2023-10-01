namespace Bazaar.Catalog.DTOs;

public class CatalogItemCreateCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }
    public string SellerId { get; set; }

    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }

    public CatalogItem ToCatalogItem() => new()
    {
        Name = Name,
        Description = Description,
        Price = Price,
        AvailableStock = AvailableStock,
        SellerId = SellerId,
        RestockThreshold = RestockThreshold,
        MaxStockThreshold = MaxStockThreshold
    };
}
