namespace WebShoppingUI.DataServices;

public interface IBasketDataService
{
    Task<ServiceCallResult<Basket>> GetBasketByBuyerId(string buyerId);
    Task<ServiceCallResult<Basket>> AddItemToBasket(string buyerId, BasketItem basketItem);
    Task<ServiceCallResult> ChangeItemQuantity(string buyerId, string productId, uint quantity);
    Task<ServiceCallResult> Checkout(BasketCheckout checkout);
    Task<ServiceCallResult> RemoveItemFromBasket(string buyerId, string productId);
}
