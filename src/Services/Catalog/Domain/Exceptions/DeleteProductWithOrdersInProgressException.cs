namespace Bazaar.Catalog.Domain.Exceptions;

public class DeleteProductWithOrdersInProgressException : Exception
{
    public DeleteProductWithOrdersInProgressException()
        : base("This product cannot be deleted because it has orders in progress.") { }
}
