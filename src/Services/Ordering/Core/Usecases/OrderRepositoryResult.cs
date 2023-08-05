namespace Bazaar.Ordering.Core.Usecases;

public abstract class OrderRepositoryResult
{
}

// Concrete result classes
public class OrderSuccessResult : OrderRepositoryResult, IUpdateOrderStatusResult
{
    public Order Order;

    public OrderSuccessResult(Order order)
    {
        Order = order;
    }
}
public class OrderNotFoundError : OrderRepositoryResult, IUpdateOrderStatusResult { }
public class InvalidOrderCancellationError : OrderRepositoryResult, IUpdateOrderStatusResult
{
    public string Error;

    public InvalidOrderCancellationError(string error)
    {
        Error = error;
    }
}

// Method return interfaces
public interface IUpdateOrderStatusResult
{
    static OrderSuccessResult Success(Order order) => new(order);
    static OrderNotFoundError OrderNotFoundError => new();
    static InvalidOrderCancellationError InvalidOrderCancellationError =>
        new("Attempted to cancel an order that is either " +
                "(1) processing payment, " +
                "(2) shipping, " +
                "(3) shipped or " +
                "(4) already cancelled");
}
