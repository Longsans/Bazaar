namespace Bazaar.Payment.Events;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int OrderId) : IntegrationEvent;