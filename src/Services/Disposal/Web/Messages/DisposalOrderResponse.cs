namespace Bazaar.Disposal.Web.Messages;

public record DisposalOrderResponse(
    int Id,
    IReadOnlyCollection<DisposeQuantityResponse> DisposeQuantities,
    DateTime CreatedAt,
    bool CreatedByBazaar,
    DisposalStatus Status,
    string? CancelReason)
{
    public DisposalOrderResponse(DisposalOrder disposalOrder)
        : this(disposalOrder.Id,
              disposalOrder.DisposeQuantities
                .Select(x => new DisposeQuantityResponse(x)).ToList(),
              disposalOrder.CreatedAt, disposalOrder.CreatedByBazaar,
              disposalOrder.Status, disposalOrder.CancelReason)
    {

    }
}
