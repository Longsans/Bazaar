namespace Bazaar.FbbInventory.Web.Messages;

public record FulfillableLotResponse(
    int Id,
    DateTime DateEnteredStorage,
    uint UnitsInStock,
    uint UnitsPendingRemoval,
    uint TotalUnits,
    int ProductInventoryId
)
{
    public FulfillableLotResponse(FulfillableLot qty)
        : this(qty.Id, qty.DateEnteredStorage, qty.UnitsInStock,
              qty.UnitsPendingRemoval, qty.TotalUnits, qty.ProductInventoryId)
    {

    }
}
