namespace Bazaar.Ordering.ServiceIntegration.EventHandling;

public class DeliveryStatusChangedIntegrationEventHandler
    : IIntegrationEventHandler<DeliveryStatusChangedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IHandleOrderService _handleOrderService;

    public DeliveryStatusChangedIntegrationEventHandler(
        IOrderRepository orderRepo, IHandleOrderService handleOrderService)
    {
        _orderRepo = orderRepo;
        _handleOrderService = handleOrderService;
    }

    public async Task Handle(DeliveryStatusChangedIntegrationEvent @event)
    {
        if (@event.Status == DeliveryStatus.Scheduled
            || @event.Status == DeliveryStatus.Delivering)
        {
            return;
        }

        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            // This should never happen unless
            // the delivery service received a wrong order ID at first
            return;
        }

        var correspondingOrderStatus = @event.Status switch
        {
            DeliveryStatus.Completed => OrderStatus.Shipped,
            DeliveryStatus.Postponed => OrderStatus.Postponed,
            DeliveryStatus.Cancelled => OrderStatus.Cancelled,
        };
        _handleOrderService.UpdateOrderStatus(order.Id, correspondingOrderStatus);

        await Task.CompletedTask;
    }
}
