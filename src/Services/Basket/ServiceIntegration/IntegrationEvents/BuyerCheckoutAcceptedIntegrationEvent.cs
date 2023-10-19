namespace Bazaar.Basket.ServiceIntegration.IntegrationEvents;

public record BuyerCheckoutAcceptedIntegrationEvent : IntegrationEvent
{
    public string City { get; init; }
    public string Country { get; init; }
    public string ZipCode { get; init; }
    public string ShippingAddress { get; init; }

    public string CardNumber { get; init; }
    public string CardHolderName { get; init; }
    public DateTime CardExpiration { get; init; }
    public string CardSecurityNumber { get; init; }

    public string BuyerId { get; init; }
    public CheckoutEventBasket Basket { get; init; }

    public BuyerCheckoutAcceptedIntegrationEvent(
        string city, string country, string zipCode, string shippingAdress,
        string cardNumber, string cardHolderName,
        DateTime cardExpiration, string cardSecurityNumber,
        string buyerId, BuyerBasket basket)
    {
        City = city;
        Country = country;
        ZipCode = zipCode;
        ShippingAddress = shippingAdress;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
        CardSecurityNumber = cardSecurityNumber;
        BuyerId = buyerId;
        Basket = new CheckoutEventBasket(
            basket.BuyerId,
            basket.Items.Select(item => new CheckoutEventBasketItem(
                item.ProductId, item.ProductName, item.ProductUnitPrice, item.Quantity)));
    }
}

public record CheckoutEventBasket(string BuyerId, IEnumerable<CheckoutEventBasketItem> Items);

public record CheckoutEventBasketItem(string ProductId, string ProductName, decimal UnitPrice, uint Quantity);
