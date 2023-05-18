namespace Bazaar.Basket.Model;

public interface IBasketRepository
{
    CustomerBasket? GetByBuyerId(string buyerId);
}