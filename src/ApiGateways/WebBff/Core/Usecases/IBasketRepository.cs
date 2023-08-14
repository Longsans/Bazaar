namespace Bazaar.ApiGateways.WebBff.Core.Usecases;

public interface IBasketRepository
{
    Task<Basket?> GetByBuyerId(string buyerId);
    Task<Basket?> AddItemToBasket(string buyerId, BasketItem item);
    Task<BasketItem?> ChangeItemQuantity(string buyerId, string productId, uint quantity);
}
