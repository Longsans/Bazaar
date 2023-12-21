namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCompletedIntegrationEvent(
    int ReturnId, IEnumerable<LotQuantity> ReturnedLotQuantities) : IntegrationEvent;