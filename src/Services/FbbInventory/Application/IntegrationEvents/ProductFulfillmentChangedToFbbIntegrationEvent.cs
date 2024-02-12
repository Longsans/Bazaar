namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductFulfillmentChangedToFbbIntegrationEvent(
    string ProductId, float ProductLengthInCm, float ProductWidthInCm, float ProductHeightInCm, string SellerId) : IntegrationEvent;
