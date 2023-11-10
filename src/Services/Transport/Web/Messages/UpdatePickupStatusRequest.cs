namespace Bazaar.Transport.Web.Messages;

public record UpdatePickupStatusRequest(InventoryPickupStatus Status, string? CancelReason);
