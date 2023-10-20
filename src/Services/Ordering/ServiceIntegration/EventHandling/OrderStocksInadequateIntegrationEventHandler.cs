namespace Bazaar.Ordering.ServiceIntegration.EventHandling;

public class OrderStocksInadequateIntegrationEventHandler : IIntegrationEventHandler<OrderStocksInadequateIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<OrderStocksInadequateIntegrationEventHandler> _logger;

    public OrderStocksInadequateIntegrationEventHandler(
        IOrderRepository orderRepo, ILogger<OrderStocksInadequateIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepo;
        _logger = logger;
    }

    public async Task Handle(OrderStocksInadequateIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.OrderId);
        if (order == null)
        {
            _logger.LogWarning(
                $"Event handler for [StocksInadequateEvent]: Order {@event.OrderId} no longer exists in the system.");
            return;
        }

        _logger.LogCritical(
            $"Event handler for [StocksInadequateEvent]: Order {order.Id} has been removed from database. " +
            $"Items reported with inadequate stocks: " +
            $"{string.Join(", ", @event.OrderStockInadequateItems.Select(i => i.ProductId))}");

        _orderRepo.Delete(order);
        await Task.CompletedTask;
    }
}
