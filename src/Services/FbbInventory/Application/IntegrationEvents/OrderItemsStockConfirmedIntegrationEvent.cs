namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record OrderItemsStockConfirmedIntegrationEvent(
    int OrderId, IEnumerable<string> ConfirmedProductIds) : IntegrationEvent;