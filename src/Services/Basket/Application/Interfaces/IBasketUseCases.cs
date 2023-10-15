namespace Bazaar.Basket.Application.Interfaces;

public interface IBasketUseCases
{
    BuyerBasket GetBasketOrCreateIfNotExist(string buyerId);
    BasketItem? GetBasketItem(string buyerId, string productId);

    Result<BuyerBasket> AddItemToBasket(string buyerId, BasketItemDto item);

    Result<BuyerBasket> ChangeItemQuantity(
        string buyerId, string productId, uint quantity);

    Result Checkout(BasketCheckout checkout);
}
