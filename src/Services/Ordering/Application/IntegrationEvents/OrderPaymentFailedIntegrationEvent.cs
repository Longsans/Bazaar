namespace Bazaar.Ordering.Application.IntegrationEvents;

public record OrderPaymentFailedIntegrationEvent(int orderId) : IntegrationEvent;