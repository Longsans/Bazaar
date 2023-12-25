namespace Bazaar.Basket.Application.IntegrationEvents;

public record BasketCheckoutAcceptedIntegrationEvent : IntegrationEvent
{
    public string BuyerId { get; }

    public string ShippingAddress { get; }
    public string City { get; }
    public string Country { get; }
    public string ZipCode { get; }
    public IEnumerable<CheckoutEventBasketItem> BasketItems { get; }

    public BasketCheckoutAcceptedIntegrationEvent(
        string buyerId, string shippingAddress, string city,
        string country, string zipCode, BuyerBasket basket)
    {
        BuyerId = buyerId;
        City = city;
        Country = country;
        ZipCode = zipCode;
        ShippingAddress = shippingAddress;
        BasketItems = basket.Items.Select(item => new CheckoutEventBasketItem(
                item.ProductId, item.ProductName,
                item.ProductUnitPrice, item.Quantity));
    }
}

public record CheckoutEventBasketItem(
    string ProductId, string ProductName, decimal ProductUnitPrice, uint Quantity);
