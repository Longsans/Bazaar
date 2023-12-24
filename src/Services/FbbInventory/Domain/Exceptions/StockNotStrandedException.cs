namespace Bazaar.FbbInventory.Domain.Exceptions;

public class StockNotStrandedException : Exception
{
    public StockNotStrandedException() : base("Product stock is not stranded.")
    {

    }

    public StockNotStrandedException(string message) : base(message) { }
}
