namespace Bazaar.Ordering.Domain.Exceptions;

public class OrderNotCancellableException
    : Exception
{
    public OrderNotCancellableException()
        : base("Order can only be cancelled if currently awaiting seller's confirmation or being postponed.")
    { }
}
