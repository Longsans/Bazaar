namespace Bazaar.Disposal.ServiceIntegration.IntegrationEvents;

public record LotUnitsLabeledForDisposalIntegrationEvent(
    IEnumerable<LotUnitsMarkedForDisposal> DisposeLotUnits,
    bool LabeledByBazaar) : IntegrationEvent;

public record LotUnitsMarkedForDisposal(
    string LotNumber, uint UnitsToDispose, string InventoryOwnerId);
