namespace Bazaar.Ordering.Core.Model;

public enum OrderStatus
{
    ProcessingPayment,
    Shipping,
    Shipped,
    Cancelled,
    Postponed,
}