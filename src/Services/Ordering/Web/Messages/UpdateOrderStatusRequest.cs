namespace Bazaar.Ordering.Web.Messages;

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
}
