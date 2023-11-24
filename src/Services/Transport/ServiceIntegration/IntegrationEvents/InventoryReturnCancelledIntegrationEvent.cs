namespace Bazaar.Transport.ServiceIntegration.IntegrationEvents;

public record InventoryReturnCancelledIntegrationEvent(
    int ReturnId, IEnumerable<UnitsFromLot> LotsWithUnreturnedUnits) : IntegrationEvent;
