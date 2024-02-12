namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductFulfillmentMethodChangedToFbbIntegrationEvent(
    string ProductId, float ProductLengthInCm, float ProductWidthInCm, float ProductHeightInCm, string SellerId) : IntegrationEvent;
