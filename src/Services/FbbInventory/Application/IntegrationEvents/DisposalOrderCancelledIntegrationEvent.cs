namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record DisposalOrderCancelledIntegrationEvent(
    int DisposalOrderId, IEnumerable<UndisposedQuantity> UndisposedQuantities,
    DateTime CancelledAt) : IntegrationEvent;

public record UndisposedQuantity(string LotNumber, uint UndisposedUnits);