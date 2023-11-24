namespace Bazaar.FbbInventory.Domain.Exceptions;

public class NotEnoughStockException : Exception
{
    public NotEnoughStockException()
        : base("Product does not have enough stock.") { }

    public NotEnoughStockException(string message)
        : base(message) { }
}
