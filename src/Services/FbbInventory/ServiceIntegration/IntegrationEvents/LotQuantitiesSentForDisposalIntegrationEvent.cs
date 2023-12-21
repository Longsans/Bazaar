namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record LotQuantitiesSentForDisposalIntegrationEvent(
    IEnumerable<DisposalLotQuantity> DisposalLotQuantities,
    bool IsInitiatedByBazaar) : IntegrationEvent;

public record DisposalLotQuantity(
    string LotNumber, uint DisposalQuantity, string InventoryOwnerId);
