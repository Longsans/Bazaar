namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCancelledIntegrationEvent(
    int ReturnId, IEnumerable<UnitsFromLot> LotsWithUnreturnedUnits) : IntegrationEvent;
