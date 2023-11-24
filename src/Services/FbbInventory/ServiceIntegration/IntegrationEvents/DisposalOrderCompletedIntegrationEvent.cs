namespace Bazaar.FbbInventory.ServiceIntegration.IntegrationEvents;

public record DisposalOrderCompletedIntegrationEvent(
    int DisposalOrderId, IEnumerable<DisposedQuantity> DisposedQuantities,
    DateTime CompletedAt) : IntegrationEvent;

public record DisposedQuantity(string LotNumber, uint DisposedUnits);