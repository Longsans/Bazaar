namespace WebShoppingUI.Managers;

public class BasketManager
{
    private readonly IBasketDataService _basketService;
    private readonly ICatalogDataService _catalogService;

    public BasketManager(
        IBasketDataService basketService, ICatalogDataService catalogService)
    {
        _basketService = basketService;
        _catalogService = catalogService;
    }

    public async Task<ServiceCallResult<Basket>> GetBasketByBuyerId(string buyerId)
    {
        return await _basketService.GetBasketByBuyerId(buyerId);
    }

    public async Task<ServiceCallResult<Basket>> AddItemToBasket(string buyerId, BasketItem basketItem)
    {
        var catalogItemResult = await _catalogService.GetByProductId(basketItem.ProductId);
        if (!catalogItemResult.IsSuccess)
            return catalogItemResult.WithValue<Basket>();

        var catalogItem = catalogItemResult.Value!;
        if (catalogItem.AvailableStock < basketItem.Quantity)
        {
            return ServiceCallResult<Basket>.BadRequest(
                $"Catalog item {basketItem.ProductId} does not have enough stock to satisfy request.");
        }

        basketItem.ProductName = catalogItem.ProductName;
        basketItem.UnitPrice = catalogItem.Price;
        basketItem.ImageUrl = catalogItem.ImageUrl;
        var basketItemResult = await _basketService.AddItemToBasket(buyerId, basketItem);
        return basketItemResult;
    }

    public async Task<ServiceCallResult> ChangeItemQuantity(
        string buyerId, string productId, uint quantity)
    {
        var catalogItemResult = await _catalogService.GetByProductId(productId);
        if (!catalogItemResult.IsSuccess)
            return catalogItemResult;

        if (catalogItemResult.Value!.AvailableStock < quantity)
        {
            return ServiceCallResult.BadRequest(
                $"Catalog item {productId} does not have enough stock to satisfy request.");
        }

        var basketItemResult = await _basketService.ChangeItemQuantity(buyerId, productId, quantity);
        if (!basketItemResult.IsSuccess)
        {
            return basketItemResult;
        }

        return ServiceCallResult.Success;
    }

    public async Task<ServiceCallResult> Checkout(BasketCheckout checkout)
    {
        return await _basketService.Checkout(checkout);
    }
}
