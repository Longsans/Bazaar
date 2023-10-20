namespace Bazaar.Basket.Domain.Exceptions;

public class ProductAlreadyInBasketException : Exception
{
    public ProductAlreadyInBasketException()
        : base("Basket already has this product.") { }
}
