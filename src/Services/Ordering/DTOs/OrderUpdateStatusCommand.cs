namespace Bazaar.Ordering.DTOs;

public class OrderUpdateStatusCommand
{
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
}
