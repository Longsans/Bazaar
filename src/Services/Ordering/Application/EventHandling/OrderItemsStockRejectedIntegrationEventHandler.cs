namespace Bazaar.Ordering.Application.EventHandling;

public class OrderItemsStockRejectedIntegrationEventHandler
    : IIntegrationEventHandler<OrderItemsStockRejectedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly HandleOrderService _handleOrderService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderItemsStockRejectedIntegrationEventHandler> _logger;

    public OrderItemsStockRejectedIntegrationEventHandler(
        IOrderRepository orderRepository,
        HandleOrderService handleOrderService,
        IEventBus eventBus,
        ILogger<OrderItemsStockRejectedIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepository;
        _handleOrderService = handleOrderService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task Handle(OrderItemsStockRejectedIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            _logger.LogError(
                "Error setting rejected stocks for order {OrderId}: " +
                "Order no longer exists in the system.", @event.OrderId);
            return;
        }
        if (!@event.StockRejectedItems.Any())
        {
            _logger.LogError("Error setting rejected stocks for order {OrderId}: " +
                "Stock rejected items list empty.", order.Id);
            return;
        }

        foreach (var rejectedItem in @event.StockRejectedItems)
        {
            var orderItem = order.Items.SingleOrDefault(x => x.ProductId == rejectedItem.ProductId);
            if (orderItem == null)
            {
                _logger.LogError("Error setting rejected stocks for order {OrderId}: " +
                    "Product {ProductId} is not in order items list.",
                    order.Id, rejectedItem.ProductId);
                return;
            }
            orderItem.SetStockRejected();
        }

        _handleOrderService.DeleteOrder(order.Id);
        _eventBus.Publish(new OrderRejectedIntegrationEvent(
            order.Id, order.BuyerId, @event.StockRejectedItems));
        await Task.CompletedTask;
    }
}
