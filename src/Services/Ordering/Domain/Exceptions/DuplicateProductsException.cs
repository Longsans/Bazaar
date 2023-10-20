namespace Bazaar.Ordering.Domain.Exceptions;

public class DuplicateProductsException : Exception
{
    public readonly IEnumerable<string> ProductIds;

    public DuplicateProductsException() : base("Order contains duplicate products.") { }

    public DuplicateProductsException(IEnumerable<string> productIds)
        : base($"Order contains duplicate products. Duplicate product ID's: {string.Join(", ", productIds)}")
    {
        ProductIds = productIds;
    }
}
