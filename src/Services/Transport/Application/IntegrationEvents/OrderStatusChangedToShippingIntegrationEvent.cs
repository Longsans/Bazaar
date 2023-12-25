namespace Bazaar.Transport.Application.IntegrationEvents;

public record OrderStatusChangedToShippingIntegrationEvent(
    int OrderId, string ShippingAddress, IEnumerable<ShippingOrderItem> OrderItems) : IntegrationEvent;

public record ShippingOrderItem(string ProductId, uint Quantity);