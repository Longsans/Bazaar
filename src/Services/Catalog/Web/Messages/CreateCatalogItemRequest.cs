namespace Bazaar.Catalog.Web.Messages;

public class CreateCatalogItemRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public IFormFile Image { get; set; }
    public uint AvailableStock { get; set; }
    public int CategoryId { get; set; }
    public string SellerId { get; set; }
    public FulfillmentMethod FulfillmentMethod { get; set; }
}
