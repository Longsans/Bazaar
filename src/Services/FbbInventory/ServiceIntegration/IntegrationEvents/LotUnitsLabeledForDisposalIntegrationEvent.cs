namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record LotUnitsLabeledForDisposalIntegrationEvent(
    IEnumerable<LotUnitsLabeledForDisposal> DisposeLotUnits,
    bool LabeledByBazaar) : IntegrationEvent;

public record LotUnitsLabeledForDisposal(
    string LotNumber, uint UnitsToDispose, string InventoryOwnerId);
