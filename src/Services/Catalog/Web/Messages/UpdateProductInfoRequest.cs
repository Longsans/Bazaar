namespace Bazaar.Catalog.Web.Messages;

public record UpdateProductInfoRequest
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal? Price { get; private set; }
    public string? ImageUrl { get; private set; }
    public CatalogItemDimensionsRequest? Dimensions { get; private set; }
}
