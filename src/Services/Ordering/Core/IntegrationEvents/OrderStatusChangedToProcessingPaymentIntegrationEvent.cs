namespace Bazaar.Ordering.Core.IntegrationEvents;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int orderId) : IntegrationEvent;