namespace Bazaar.Payment.Application.EventHandling;

public class OrderStatusChangedToProcessingPaymentIntegrationEventHandler
    : IIntegrationEventHandler<OrderStatusChangedToProcessingPaymentIntegrationEvent>
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStatusChangedToProcessingPaymentIntegrationEventHandler> _logger;

    public OrderStatusChangedToProcessingPaymentIntegrationEventHandler(
        IEventBus eventBus,
        ILogger<OrderStatusChangedToProcessingPaymentIntegrationEventHandler> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToProcessingPaymentIntegrationEvent @event)
    {
        _logger.LogInformation("Processing payment for order {orderId}.", @event.OrderId);
        _eventBus.Publish(new OrderPaymentSucceededIntegrationEvent(@event.OrderId));
        _logger.LogInformation("Payment for order {orderId} processed.", @event.OrderId);
        await Task.CompletedTask;
    }
}