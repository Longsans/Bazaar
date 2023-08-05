namespace Bazaar.Basket.Core.Usecases;

public interface IBasketRepository
{
    BuyerBasket? GetByBuyerId(string buyerId);
    BuyerBasket GetBasketOrCreateIfNotExist(string buyerId);
    BuyerBasket? AddItemToBasket(string buyerId, BasketItem item);
    IChangeItemQuantityResult ChangeItemQuantity(string buyerId, string productId, uint quantity);
    IRemoveItemFromBasketResult RemoveItemFromBasket(string buyerId, string productId);
}