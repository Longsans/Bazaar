namespace Bazaar.Ordering.ServiceIntegration.IntegrationEvents;

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
}

public record CheckoutEventBasket(string BuyerId, IEnumerable<CheckoutEventBasketItem> Items);

public record CheckoutEventBasketItem(string ProductId, string ProductName, decimal ProductUnitPrice, uint Quantity);
