namespace Bazaar.Payment.EventHandling;

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
        _logger.LogInformation("Payment requested, processing...");
        _eventBus.Publish(new OrderPaymentSucceededIntegrationEvent(@event.OrderId));
        await Task.CompletedTask;
    }
}