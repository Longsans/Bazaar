namespace Bazaar.Catalog.Application.IntegrationEvents;

public record CatalogItemCreatedIntegrationEvent(
    string ProductId, string ProductName, string ProductDescription, decimal Price, string? ImageFilename,
    decimal ProductLengthCm, decimal ProductWidthCm, decimal ProductHeightCm, uint AvailableStock,
    int MainDepartmentId, int SubcategoryId, string SellerId, ListingStatus ListingStatus, FulfillmentMethod FulfillmentMethod) : IntegrationEvent
{
    public CatalogItemCreatedIntegrationEvent(CatalogItem item)
        : this(item.ProductId, item.ProductName, item.ProductDescription, item.Price, item.ImageFilename,
              item.ProductLengthCm, item.ProductWidthCm, item.ProductHeightCm, item.AvailableStock,
              item.MainDepartmentId, item.SubcategoryId, item.SellerId, item.ListingStatus, item.FulfillmentMethod)
    {
    }
}