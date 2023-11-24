namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCompletedIntegrationEvent(
    int ReturnId, IEnumerable<UnitsFromLot> LotsWithReturnedUnits) : IntegrationEvent;