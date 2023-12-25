namespace Bazaar.Ordering.Application.IntegrationEvents;

public record BasketCheckoutAcceptedIntegrationEvent
(
    string BuyerId,
    string ShippingAddress,
    string City,
    string Country,
    string ZipCode,
    IEnumerable<CheckoutEventBasketItem> BasketItems
) : IntegrationEvent;


public record CheckoutEventBasketItem(
    string ProductId, string ProductName,
    decimal ProductUnitPrice, uint Quantity);
