namespace Bazaar.Catalog.Domain.Exceptions;

public class ManualFbbStockManagementNotSupportedException : Exception
{
    public ManualFbbStockManagementNotSupportedException()
        : base("Cannot manually update FBB stocks. FBB stocks are managed by Bazaar.")
    { }
}
