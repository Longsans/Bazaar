namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record LotUnitsLabeledForReturnIntegrationEvent(
    IEnumerable<LotUnitsLabeledForReturn> ReturnLotUnits) : IntegrationEvent;

public record LotUnitsLabeledForReturn(
    string LotNumber, uint UnitsToReturn, string InventoryOwnerId);