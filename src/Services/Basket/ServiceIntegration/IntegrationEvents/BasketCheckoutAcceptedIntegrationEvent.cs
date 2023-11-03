namespace Bazaar.Basket.ServiceIntegration.IntegrationEvents;

public record BasketCheckoutAcceptedIntegrationEvent : IntegrationEvent
{
    public string BuyerId { get; init; }
    public IEnumerable<CheckoutEventBasketItem> BasketItems { get; init; }

    public BasketCheckoutAcceptedIntegrationEvent(
        string buyerId, BuyerBasket basket)
    {
        BuyerId = buyerId;
        BasketItems = basket.Items.Select(item => new CheckoutEventBasketItem(
                item.ProductId, item.ProductName,
                item.ProductUnitPrice, item.Quantity));
    }
}

public record CheckoutEventBasketItem(
    string ProductId, string ProductName, decimal UnitPrice, uint Quantity);
