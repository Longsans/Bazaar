namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private IOrderRepository _orderRepo;

    public OrderPaymentFailedIntegrationEventHandler(IOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        _orderRepo.UpdateStatus(@event.orderId, OrderStatus.Postponed);
        await Task.CompletedTask;
    }
}