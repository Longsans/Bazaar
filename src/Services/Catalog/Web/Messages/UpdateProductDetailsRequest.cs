namespace Bazaar.Catalog.Web.Messages;

public record UpdateProductDetailsRequest
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public string? ImageUrl { get; private set; }
}
