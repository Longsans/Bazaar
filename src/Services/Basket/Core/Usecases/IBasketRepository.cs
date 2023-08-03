namespace Bazaar.Basket.Core.Usecases;

public interface IBasketRepository
{
    BuyerBasket? GetByBuyerId(string buyerId);
    BuyerBasket GetBasketOrCreateIfNotExist(string buyerId);
    BuyerBasket? AddItemToBasket(string buyerId, BasketItem item);
    BuyerBasket? ChangeItemQuantity(string buyerId, string productId, uint quantity);
    BuyerBasket? RemoveItemFromBasket(string buyerId, string productId);
}