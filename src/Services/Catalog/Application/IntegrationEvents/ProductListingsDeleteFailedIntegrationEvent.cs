namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductListingsDeleteFailedIntegrationEvent(
    IEnumerable<FailedDeleteListing> FailedListings, string SellerId) : IntegrationEvent;

public record FailedDeleteListing(string ProductId, ListingDeleteFailureReason FailureReason);

public enum ListingDeleteFailureReason
{
    HasOrdersInProgress,
    HasFbbStock
}