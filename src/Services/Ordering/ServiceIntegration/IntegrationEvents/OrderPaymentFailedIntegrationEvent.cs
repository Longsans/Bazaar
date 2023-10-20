namespace Bazaar.Ordering.ServiceIntegration.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;