namespace Bazaar.Ordering.DTOs;

public record CheckoutEventBasket(string BuyerId, IEnumerable<CheckoutEventBasketItem> Items);

public record CheckoutEventBasketItem(string ProductId, string ProductName, decimal UnitPrice, uint Quantity);