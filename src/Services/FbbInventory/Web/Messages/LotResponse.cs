﻿namespace Bazaar.FbbInventory.Web.Messages;

public record LotResponse(
    int Id,
    string LotNumber,
    int ProductInventoryId,
    uint UnitsInStock,
    uint UnitsInRemoval,
    uint TotalUnits,
    float StorageSpaceUsedInCm3,
    float StorageSpaceUsedInM3,
    DateTime DateUnitsEnteredStorage,
    DateTime? DateUnitsBecameStranded,
    DateTime? DateUnitsBecameUnfulfillable,
    UnfulfillableCategory? UnfulfillableCategory,
    bool IsUnfulfillableBeyondPolicyDuration
)
{
    public LotResponse(Lot lot)
        : this(lot.Id, lot.LotNumber, lot.ProductInventoryId,
              lot.UnitsInStock, lot.UnitsInRemoval, lot.TotalUnits,
              lot.StorageSpaceUsedCm3, lot.StorageSpaceUsedM3,
              lot.DateUnitsEnteredStorage, lot.DateUnitsBecameStranded,
              lot.DateUnitsBecameUnfulfillable, lot.UnfulfillableCategory,
              lot.IsUnitsUnfulfillableBeyondPolicyDuration)
    {

    }
}
