namespace Bazaar.Ordering.Domain.IntegrationEvents;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int OrderId) : IntegrationEvent;