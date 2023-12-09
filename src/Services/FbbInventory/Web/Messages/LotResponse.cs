namespace Bazaar.FbbInventory.Web.Messages;

public record LotResponse(
    int Id,
    uint UnitsInStock,
    uint UnitsPendingRemoval,
    uint TotalUnits,
    DateTime TimeEnteredStorage,
    DateTime? TimeUnfulfillableSince,
    UnfulfillableCategory? UnfulfillableCategory,
    int ProductInventoryId,
    bool IsUnfulfillableBeyondPolicyDuration
)
{
    public LotResponse(Lot lot)
        : this(lot.Id, lot.UnitsInStock, lot.UnitsPendingRemoval, lot.TotalUnits,
              lot.TimeEnteredStorage, lot.TimeUnfulfillableSince, lot.UnfulfillableCategory,
              lot.ProductInventoryId, lot.IsUnfulfillableBeyondPolicyDuration)
    {

    }
}
