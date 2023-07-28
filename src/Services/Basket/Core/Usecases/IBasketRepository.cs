namespace Bazaar.Basket.Core.Usecases;

public interface IBasketRepository
{
    CustomerBasket? GetByBuyerId(string buyerId);
    void Update(string buyerId, CustomerBasket basket);
    CustomerBasket GetBasketOrCreateIfNotExist(string buyerId);
}