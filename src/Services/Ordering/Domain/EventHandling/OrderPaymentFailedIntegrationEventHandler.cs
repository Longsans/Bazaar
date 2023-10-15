namespace Bazaar.Ordering.Domain.EventHandling;

public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;

    public OrderPaymentFailedIntegrationEventHandler(IOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        var order = _orderRepo.GetById(@event.orderId);
        if (order == null)
            return;

        order.Postpone();
        _orderRepo.Update(order);
        await Task.CompletedTask;
    }
}