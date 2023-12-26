namespace Bazaar.Payment.Application.IntegrationEvents;

public record OrderStatusChangedToProcessingPaymentIntegrationEvent(int OrderId) : IntegrationEvent;