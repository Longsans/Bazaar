namespace Bazaar.Catalog.Web.Messages;

public class UpdateProductDetailsRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}
