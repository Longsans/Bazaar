namespace Bazaar.Payment.Events;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int orderId) : IntegrationEvent;