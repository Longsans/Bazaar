namespace WebShoppingUI.HttpServices;

public interface IBasketHttpService
{
    Task<ServiceCallResult<Basket>> GetBasketByBuyerId(string buyerId);
    Task<ServiceCallResult<Basket>> AddItemToBasket(string buyerId, BasketItem basketItem);
    Task<ServiceCallResult> ChangeItemQuantity(string buyerId, string productId, uint quantity);
    Task<ServiceCallResult> Checkout(BasketCheckout checkout);
}
