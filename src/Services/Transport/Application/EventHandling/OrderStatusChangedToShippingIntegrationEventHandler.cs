namespace Bazaar.Transport.Application.EventHandling;

public class OrderStatusChangedToShippingIntegrationEventHandler
    : IIntegrationEventHandler<OrderStatusChangedToShippingIntegrationEvent>
{
    private readonly DeliveryProcessService _deliveryProcessor;
    private readonly ILogger<OrderStatusChangedToShippingIntegrationEventHandler> _logger;

    public OrderStatusChangedToShippingIntegrationEventHandler(
        DeliveryProcessService deliveryProcessor,
        ILogger<OrderStatusChangedToShippingIntegrationEventHandler> logger)
    {
        _deliveryProcessor = deliveryProcessor;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToShippingIntegrationEvent @event)
    {
        var packageItems = @event.OrderItems.Select(x =>
            new DeliveryPackageItem(x.ProductId, x.Quantity));
        var result = await _deliveryProcessor.ScheduleDelivery(
            @event.OrderId, @event.ShippingAddress, packageItems);

        if (result.IsSuccess)
        {
            _logger.LogCritical("Delivery for order {orderId} scheduled. Delivery ID: {deliveryId}.",
                @event.OrderId, result.Value.Id);
        }
        else
        {
            _logger.LogError("Error scheduling delivery for order ID {orderId}: {error}",
                @event.OrderId, string.Join(", ", result.ValidationErrors.Select(x => x.ErrorMessage)));
        }
        await Task.CompletedTask;
    }
}
