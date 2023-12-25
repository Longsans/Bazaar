namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int OrderId) : IntegrationEvent;