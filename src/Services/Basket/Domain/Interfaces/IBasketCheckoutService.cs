namespace Bazaar.Basket.Domain.Interfaces;

public interface IBasketCheckoutService
{
    Result Checkout(BasketCheckout checkout);
}
