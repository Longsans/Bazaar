namespace Bazaar.Catalog.Domain.Exceptions;

public class ExceedingMaxStockThresholdException
    : Exception
{
    public ExceedingMaxStockThresholdException()
        : base("Available stock cannot exceed max stock threshold.")
    { }
}
