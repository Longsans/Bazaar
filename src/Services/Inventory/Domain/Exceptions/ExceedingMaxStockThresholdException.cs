namespace Bazaar.Inventory.Domain.Exceptions;

public class ExceedingMaxStockThresholdException
    : Exception
{
    public ExceedingMaxStockThresholdException()
        : base("Units in stock cannot exceed max stock threshold.")
    { }
}
