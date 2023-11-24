namespace Bazaar.Transport.Web.Messages;

public readonly record struct UpdateInventoryReturnStatusRequest(
    DeliveryStatus Status, string? CancelReason = null);
