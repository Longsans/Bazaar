namespace Bazaar.Transport.Application.EventHandling;

public class OrderStatusChangedToShippingIntegrationEventHandler
    : IIntegrationEventHandler<OrderStatusChangedToShippingIntegrationEvent>
{
    private readonly DeliveryProcessService _deliveryProcessService;
    private readonly ILogger<OrderStatusChangedToShippingIntegrationEventHandler> _logger;

    public OrderStatusChangedToShippingIntegrationEventHandler(
        DeliveryProcessService deliveryProcessService,
        ILogger<OrderStatusChangedToShippingIntegrationEventHandler> logger)
    {
        _deliveryProcessService = deliveryProcessService;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToShippingIntegrationEvent @event)
    {
        var packageItems = @event.OrderItems.Select(x =>
            new DeliveryPackageItem(x.ProductId, x.Quantity));
        var result = await _deliveryProcessService.ScheduleDelivery(
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
