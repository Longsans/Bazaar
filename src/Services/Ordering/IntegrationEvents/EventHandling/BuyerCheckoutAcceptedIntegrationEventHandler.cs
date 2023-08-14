namespace Bazaar.Ordering.IntegrationEvents.EventHandling;

public class BuyerCheckoutAcceptedIntegrationEventHandler : IIntegrationEventHandler<BuyerCheckoutAcceptedIntegrationEvent>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IEventBus _eventBus;

    public BuyerCheckoutAcceptedIntegrationEventHandler(IOrderRepository orderRepo, IEventBus eventBus)
    {
        _orderRepo = orderRepo;
        _eventBus = eventBus;
    }

    public async Task Handle(BuyerCheckoutAcceptedIntegrationEvent @event)
    {
        var order = new Order
        {
            BuyerId = @event.BuyerId,
            Items = @event.Basket.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductUnitPrice = item.UnitPrice,
                Quantity = (int)item.Quantity,
            }).ToList(),
            ShippingAddress = string.Join(", ", @event.ShippingAddress, @event.City, @event.Country),
        };
        var createResult = _orderRepo.Create(order);

        if (createResult is OrderHasNoItemsError)
        {
            return;
        }
        _eventBus.Publish(
                new OrderCreatedIntegrationEvent(
                    order.Id, order.Items.Select(item => new OrderStockItem(item.ProductId, item.Quantity))));
        await Task.CompletedTask;
    }
}
