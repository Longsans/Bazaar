namespace Bazaar.FbbInventory.Domain.Exceptions;

public class NotEnoughStockException : Exception
{
    public NotEnoughStockException()
        : base("Product does not have enough stock.") { }
}
