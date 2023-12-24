namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record InventoryReturnCompletedIntegrationEvent(
    int ReturnId, IEnumerable<LotQuantity> ReturnedLotQuantities) : IntegrationEvent;