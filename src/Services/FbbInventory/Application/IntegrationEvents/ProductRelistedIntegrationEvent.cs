namespace Bazaar.FbbInventory.Application.IntegrationEvents;

public record ProductRelistedIntegrationEvent(string ProductId) : IntegrationEvent;
