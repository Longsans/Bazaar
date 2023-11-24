namespace Bazaar.Transport.Web.Messages;

public record InventoryReturnResponse(
    int Id,
    string DeliveryAddress,
    IReadOnlyCollection<ReturnQuantityResponse> ReturnQuantities,
    DateTime TimeScheduledAt,
    DateTime EstimatedDeliveryTime,
    DeliveryStatus Status,
    string? CancelReason,
    string InventoryOwnerId)
{
    public InventoryReturnResponse(InventoryReturn invReturn)
        : this(invReturn.Id, invReturn.DeliveryAddress,
              invReturn.ReturnQuantities.Select(x => new ReturnQuantityResponse(x)).ToList(),
              invReturn.TimeScheduledAt, invReturn.EstimatedDeliveryTime, invReturn.Status,
              invReturn.CancelReason, invReturn.InventoryOwnerId)
    {

    }
}
