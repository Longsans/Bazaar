namespace Bazaar.FbbInventory.Domain.Exceptions;

public class StockStrandedException : Exception
{
    public StockStrandedException()
        : base("Product stock is stranded. Cannot add or remove stock until stranded status is fixed.")
    {

    }

    public StockStrandedException(string message) : base(message) { }
}
