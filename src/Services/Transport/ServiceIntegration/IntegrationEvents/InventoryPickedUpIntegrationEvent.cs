namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record InventoryPickedUpIntegrationEvent(
    IEnumerable<PickupProductInventory> Inventories, string SchedulerId) : IntegrationEvent;

public record PickupProductInventory(string ProductId, uint StockUnits);