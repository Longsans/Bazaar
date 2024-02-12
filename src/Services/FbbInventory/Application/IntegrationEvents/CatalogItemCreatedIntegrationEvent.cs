namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record CatalogItemCreatedIntegrationEvent(
    string ProductId, string ProductName, string ProductDescription, decimal Price, string? ImageFilename,
    float ProductLengthCm, float ProductWidthCm, float ProductHeightCm, uint AvailableStock,
    int MainDepartmentId, int SubcategoryId, string SellerId, ListingStatus ListingStatus, FulfillmentMethod FulfillmentMethod) : IntegrationEvent;

public enum ListingStatus
{
    Active,
    InactiveOutOfStock,
    InactiveClosedListing,
    Deleted
}

public enum FulfillmentMethod
{
    Merchant,
    Fbb
}
