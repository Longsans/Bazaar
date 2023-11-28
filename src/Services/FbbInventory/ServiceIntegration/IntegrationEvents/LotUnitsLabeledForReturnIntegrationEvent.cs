namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record LotUnitsLabeledForReturnIntegrationEvent(
    IEnumerable<UnitsFromLot> UnitsFromLots,
    string DeliveryAddress, string InventoryOwnerId) : IntegrationEvent;

public record UnitsFromLot(
    string LotNumber, uint Units);