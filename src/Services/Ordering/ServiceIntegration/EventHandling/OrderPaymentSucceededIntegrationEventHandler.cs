namespace Bazaar.Ordering.ServiceIntegration.EventHandling;

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
        _logger.LogInformation("Payment success, now waiting for seller's confirmation.");

        var order = _orderRepo.GetById(@event.orderId);
        if (order == null)
            return;

        order.AwaitSellerConfirmation();
        _orderRepo.Update(order);
        await Task.CompletedTask;
    }
}