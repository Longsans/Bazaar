namespace Bazaar.Ordering.Core.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;