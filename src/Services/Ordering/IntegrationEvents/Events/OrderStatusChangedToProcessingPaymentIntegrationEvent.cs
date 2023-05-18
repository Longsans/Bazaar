namespace Bazaar.Ordering.Events;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int orderId) : IntegrationEvent;