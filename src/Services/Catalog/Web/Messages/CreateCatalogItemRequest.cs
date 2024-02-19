namespace Bazaar.Catalog.Web.Messages;

public record CreateCatalogItemRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile Image { get; set; }
    public uint AvailableStock { get; set; }
    public float ProductLength { get; set; }
    public float ProductWidth { get; set; }
    public float ProductHeight { get; set; }
    public int SubcategoryId { get; set; }
    public string SellerId { get; set; }
    public FulfillmentMethod FulfillmentMethod { get; set; }
}
