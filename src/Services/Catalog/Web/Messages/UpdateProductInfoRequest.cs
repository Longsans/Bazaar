namespace Bazaar.Catalog.Web.Messages;

public record UpdateProductInfoRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? ImageUrl { get; set; }
    public CatalogItemDimensions? Dimensions { get; set; }
}
