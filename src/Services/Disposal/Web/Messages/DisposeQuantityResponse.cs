namespace Bazaar.Disposal.Web.Messages;

public record DisposeQuantityResponse(
    int Id,
    string LotNumber,
    uint UnitsToDispose,
    int DisposalOrderId,
    string InventoryOwnerId)
{
    public DisposeQuantityResponse(DisposalQuantity disposeQty)
        : this(disposeQty.Id, disposeQty.LotNumber, disposeQty.UnitsToDispose,
              disposeQty.DisposalOrderId, disposeQty.InventoryOwnerId)
    {

    }
}
