namespace Bazaar.Catalog.Web.Messages;

public record UpdateProductDetailsRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
}
