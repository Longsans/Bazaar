namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record ProductFbbInventoryUpdatedIntegrationEvent(
    string ProductId, uint FulfillableStock, uint UnfulfillableStock,
    uint FulfillableUnitsPendingRemoval,
    uint UnfulfillableUnitsPendingRemoval) : IntegrationEvent
{
    public ProductFbbInventoryUpdatedIntegrationEvent(ProductInventory inventory)
        : this(inventory.ProductId, inventory.FulfillableUnitsInStock, inventory.UnfulfillableUnitsInStock,
              inventory.FulfillableUnitsPendingRemoval, inventory.UnfulfillableUnitsPendingRemoval)
    {

    }
}
