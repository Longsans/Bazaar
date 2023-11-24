namespace Bazaar.FbbInventory.Web.Messages;

public record UnfulfillableLotResponse(
    int Id,
    DateTime DateUnfulfillableSince,
    UnfulfillableCategory UnfulfillableCategory,
    uint UnitsInStock,
    uint UnitsPendingRemoval,
    uint TotalUnits,
    int ProductInventoryId
)
{
    public UnfulfillableLotResponse(UnfulfillableLot qty)
        : this(qty.Id, qty.DateUnfulfillableSince, qty.UnfulfillableCategory,
              qty.UnitsInStock, qty.UnitsPendingRemoval,
              qty.TotalUnits, qty.ProductInventoryId)
    {

    }
}