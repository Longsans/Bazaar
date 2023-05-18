namespace Bazaar.Ordering.Events;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;