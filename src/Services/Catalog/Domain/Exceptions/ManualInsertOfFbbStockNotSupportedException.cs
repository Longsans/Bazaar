namespace Bazaar.Catalog.Domain.Exceptions;

public class ManualInsertOfFbbStockNotSupportedException : Exception
{
    public ManualInsertOfFbbStockNotSupportedException()
        : base("Manually inserting FBB stock is not supported. " +
            "Make sure stock is 0 when constructing FBB catalog items.")
    { }
}
