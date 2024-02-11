namespace Bazaar.Catalog.Application.IntegrationEvents;

public record ProductFulfillmentMethodChangedToFbbIntegrationEvent(
    string ProductId, decimal ProductLengthInCm, decimal ProductWidthInCm, decimal ProductHeightInCm, string SellerId) : IntegrationEvent;
