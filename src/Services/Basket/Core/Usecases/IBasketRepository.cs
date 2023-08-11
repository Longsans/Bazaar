namespace Bazaar.Basket.Core.Usecases;

public interface IBasketRepository
{
    BuyerBasket GetBasketOrCreateIfNotExist(string buyerId);
    BasketItem? GetBasketItem(string buyerId, string productId);
    IAddItemToBasketResult AddItemToBasket(string buyerId, BasketItem item);
    IChangeItemQuantityResult ChangeItemQuantity(string buyerId, string productId, uint quantity);
    IRemoveItemFromBasketResult RemoveItemFromBasket(string buyerId, string productId);
}