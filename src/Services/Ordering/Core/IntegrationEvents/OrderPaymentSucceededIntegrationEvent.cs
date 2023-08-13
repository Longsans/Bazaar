namespace Bazaar.Ordering.Core.IntegrationEvents;

public record OrderPaymentSucceededIntegrationEvent(int orderId) : IntegrationEvent;