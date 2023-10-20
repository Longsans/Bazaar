namespace Bazaar.Ordering.ServiceIntegration.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;