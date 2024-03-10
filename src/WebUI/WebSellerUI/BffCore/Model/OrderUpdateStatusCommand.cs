namespace WebSellerUI.Model;

public class OrderUpdateStatusCommand
{
    public OrderStatus Status { get; set; }
    public string? CancelReason { get; set; }
}
