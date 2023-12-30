namespace Bazaar.Catalog.Web.Messages;

public readonly record struct ChangeListingStatusRequest(RequestedListingStatus Status);

public enum RequestedListingStatus
{
    Listed,
    Closed,
    Deleted
}