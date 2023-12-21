namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCancelledIntegrationEvent(
    int ReturnId, IEnumerable<LotQuantity> UnreturnedLotQuantities) : IntegrationEvent;
