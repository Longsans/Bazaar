namespace Bazaar.Ordering.Application.EventHandling;

public class OrderItemsStockConfirmedIntegrationEventHandler
    : IIntegrationEventHandler<OrderItemsStockConfirmedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly HandleOrderService _handleOrderService;
    private readonly ILogger<OrderItemsStockConfirmedIntegrationEventHandler> _logger;

    public OrderItemsStockConfirmedIntegrationEventHandler(
        IOrderRepository orderRepo, HandleOrderService handleOrderService,
        ILogger<OrderItemsStockConfirmedIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepo;
        _handleOrderService = handleOrderService;
        _logger = logger;
    }

    public async Task Handle(OrderItemsStockConfirmedIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            _logger.LogError(
                "Error confirming stocks for order {OrderId}: " +
                "Order no longer exists in the system.", @event.OrderId);
            return;
        }
        if (!@event.ConfirmedProductIds.Any())
        {
            _logger.LogError("Error confirming stocks for order {OrderId}: " +
                "Confirmed product IDs list empty.", order.Id);
            return;
        }

        foreach (var productId in @event.ConfirmedProductIds)
        {
            var orderItem = order.Items.SingleOrDefault(x => x.ProductId == productId);
            if (orderItem == null)
            {
                _logger.LogError("Error confirming stocks for order {OrderId}: " +
                    "Product {ProductId} is not in order items list.", @event.OrderId, productId);
                return;
            }
            orderItem.SetStockConfirmed();
        }
        if (!order.CanProceedToPayment)
        {
            _orderRepo.Update(order);
            return;
        }

        var result = _handleOrderService.UpdateOrderStatus(order.Id, OrderStatus.ProcessingPayment);
        if (result.IsSuccess)
        {
            _logger.LogCritical("All stocks confirmed for order {OrderId}. " +
                "Order proceeded to payment.", order.Id);
        }
        else
        {
            _logger.LogError("Error sending order {OrderId} to payment process: {ErrorMessage}.",
                order.Id, string.Join(", ", result.Errors));
        }
        await Task.CompletedTask;
    }
}
