namespace Bazaar.Basket.Core.Usecases;

public interface IBasketRepository
{
    CustomerBasket? GetByBuyerId(string buyerId);
}