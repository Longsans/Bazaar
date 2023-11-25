namespace Bazaar.FbbInventory.Domain.Exceptions;

public class NotEnoughUnitsException : Exception
{
    public NotEnoughUnitsException()
        : base("Product does not have enough stock.") { }

    public NotEnoughUnitsException(string message)
        : base(message) { }
}
