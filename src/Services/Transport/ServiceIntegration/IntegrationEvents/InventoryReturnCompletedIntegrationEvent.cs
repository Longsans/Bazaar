namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCompletedIntegrationEvent(
    int ReturnId, IEnumerable<UnitsFromLot> LotsWithReturnedUnits) : IntegrationEvent;