namespace Bazaar.Ordering.ServiceIntegration.EventHandling;

public class OrderItemsUnavailableIntegrationEventHandler : IIntegrationEventHandler<OrderItemsUnavailableIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<OrderItemsUnavailableIntegrationEvent> _logger;

    public OrderItemsUnavailableIntegrationEventHandler(
        IOrderRepository orderRepo, ILogger<OrderItemsUnavailableIntegrationEvent> logger)
    {
        _orderRepo = orderRepo;
        _logger = logger;
    }

    public async Task Handle(OrderItemsUnavailableIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning($"[ItemsUnavailableEvent] Event handler: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        _logger.LogCritical($"[ItemsUnavailableEvent] Event handler: Order {order.Id} has been removed from database. " +
            $"Items reported unavailable: {string.Join(", ", @event.UnavailableProductIds)}");

        _orderRepo.Delete(order);
        await Task.CompletedTask;
    }
}
