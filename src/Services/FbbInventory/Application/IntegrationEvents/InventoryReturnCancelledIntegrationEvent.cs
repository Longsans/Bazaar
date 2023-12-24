namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record InventoryReturnCancelledIntegrationEvent(
    int ReturnId, IEnumerable<LotQuantity> UnreturnedLotQuantities) : IntegrationEvent;
