namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCompletedIntegrationEvent(
    int ReturnId, IEnumerable<LotQuantity> ReturnedLotQuantities) : IntegrationEvent;