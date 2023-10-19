namespace Bazaar.Basket.Domain.Interfaces;

public interface IBasketRepository
{
    BuyerBasket? GetWithItemsByBuyerId(string buyerId);
    BasketItem? GetBasketItem(string buyerId, string productId);
    BuyerBasket Create(BuyerBasket basket);
    void Update(BuyerBasket basket);
}