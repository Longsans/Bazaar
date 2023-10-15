namespace Bazaar.Basket.Domain.Interfaces;

public interface IBasketRepository
{
    BuyerBasket? GetByBuyerId(string buyerId);
    BasketItem? GetBasketItem(string buyerId, string productId);
    BuyerBasket Create(string buyerId);
    void Update(BuyerBasket basket);

    BasketItem AddBasketItem(BasketItem item);
}