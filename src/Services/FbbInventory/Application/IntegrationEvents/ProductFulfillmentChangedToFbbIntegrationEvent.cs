namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductFulfillmentChangedToFbbIntegrationEvent(
    string ProductId, decimal ProductLengthInCm, decimal ProductWidthInCm, decimal ProductHeightInCm, string SellerId) : IntegrationEvent;
