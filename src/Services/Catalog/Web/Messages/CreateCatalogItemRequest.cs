namespace Bazaar.Catalog.Web.Messages;

public class CreateCatalogItemRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public uint AvailableStock { get; set; }
    public string SellerId { get; set; }

    public uint RestockThreshold { get; set; }
    public uint MaxStockThreshold { get; set; }

    public CatalogItem ToNewCatalogItem() => new(
        default, string.Empty, Name, Description,
        Price, AvailableStock, SellerId,
        RestockThreshold, MaxStockThreshold);
}
