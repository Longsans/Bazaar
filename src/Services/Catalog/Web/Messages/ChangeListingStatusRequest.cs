namespace Bazaar.Catalog.Web.Messages;

public readonly record struct ChangeListingStatusRequest(ListingCloseStatus Status);

public enum ListingCloseStatus
{
    Listed,
    Closed
}