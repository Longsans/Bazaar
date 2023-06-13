namespace Bazaar.Ordering.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly ILogger<OrderPaymentSucceededIntegrationEventHandler> _logger;

    public OrderPaymentSucceededIntegrationEventHandler(
        IOrderRepository orderRepo,
        ILogger<OrderPaymentSucceededIntegrationEventHandler> logger)
    {
        _orderRepo = orderRepo;
        _logger = logger;
    }

    public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
    {
        _logger.LogInformation("Payment success, proceeding to shipment...");
        _orderRepo.UpdateStatus(@event.orderId, OrderStatus.Shipping);
        await Task.CompletedTask;
    }
}