namespace Bazaar.Ordering.Model;

public enum OrderStatus
{
    ProcessingPayment,
    Shipping,
    Shipped,
    Cancelled,
    Postponed,
}