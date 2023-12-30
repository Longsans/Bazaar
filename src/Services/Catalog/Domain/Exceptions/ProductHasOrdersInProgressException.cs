namespace Bazaar.Catalog.Domain.Exceptions;

public class ProductHasOrdersInProgressException : Exception
{
    public ProductHasOrdersInProgressException()
        : base("This product cannot be deleted because it has orders in progress.") { }
}
