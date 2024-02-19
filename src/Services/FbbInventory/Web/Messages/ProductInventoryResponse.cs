namespace Bazaar.FbbInventory.Web.Messages;

public record ProductInventoryResponse(
     int Id,
     string ProductId,
     bool IsStranded,
     float StorageLengthPerUnitInCm,
     float StorageWidthPerUnitInCm,
     float StorageHeightPerUnitInCm,
     float StorageSpacePerUnitInCm3,
     float TotalStorageSpaceInCm3,
     float TotalStorageSpaceInM3,
     uint FulfillableUnits,
     uint UnfulfillableUnits,
     uint StrandedUnits,
     uint AllUnitsInRemoval,
     uint TotalUnits,
     uint RemainingCapacity,
     uint RestockThreshold,
     uint MaxStockThreshold,
     int SellerInventoryId,
     bool HasPickupsInProgress)
{
    public ProductInventoryResponse(ProductInventory inventory) : this(inventory.Id, inventory.ProductId, inventory.IsStranded,
        inventory.StorageLengthPerUnitCm, inventory.StorageWidthPerUnitCm, inventory.StorageHeightPerUnitCm, inventory.StorageSpacePerUnitCm3,
        inventory.TotalStorageSpaceCm3, inventory.TotalStorageSpaceM3, inventory.FulfillableUnits, inventory.UnfulfillableUnits, inventory.StrandedUnits,
        inventory.AllUnitsInRemoval, inventory.TotalUnits, inventory.RemainingCapacity, inventory.RestockThreshold, inventory.MaxStockThreshold,
        inventory.SellerInventoryId, inventory.HasPickupsInProgress)
    {
    }
}
