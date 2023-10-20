namespace Bazaar.Basket.Domain.Exceptions;

public class ProductNotInBasketException : Exception
{
    public ProductNotInBasketException()
        : base("Product has not been added to basket.") { }
}
