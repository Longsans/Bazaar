namespace Bazaar.Ordering.Web.Messages;

public class PlaceOrderRequest
{
    public string BuyerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
    public string ShippingAddress { get; set; }
}