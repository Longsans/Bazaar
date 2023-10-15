namespace Bazaar.Ordering.Domain.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;