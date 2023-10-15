namespace Bazaar.Basket.Application.UseCases;

public class BasketUseCases : IBasketUseCases
{
    private readonly IBasketRepository _basketRepo;
    private readonly IEventBus _eventBus;

    public BasketUseCases(IBasketRepository basketRepo, IEventBus eventBus)
    {
        _basketRepo = basketRepo;
        _eventBus = eventBus;
    }

    public BuyerBasket GetBasketOrCreateIfNotExist(string buyerId)
    {
        var basket = _basketRepo.GetByBuyerId(buyerId)
            ?? _basketRepo.Create(buyerId);

        return basket;
    }

    public BasketItem? GetBasketItem(string buyerId, string productId)
    {
        return _basketRepo.GetBasketItem(buyerId, productId);
    }

    public Result<BuyerBasket> AddItemToBasket(string buyerId, BasketItemDto itemDto)
    {
        if (itemDto.Quantity <= 0)
            return Result.Invalid(new()
            {
                new()
                {
                    Identifier = nameof(itemDto.Quantity),
                    ErrorMessage = "Quantity must be greater than or equal to 1."
                }
            });

        var basket = GetBasketOrCreateIfNotExist(buyerId);
        var existingItem = _basketRepo.GetBasketItem(buyerId, itemDto.ProductId);
        if (existingItem != null)
            return Result.Conflict("Basket already has this product.");

        var basketItem = new BasketItem(
            itemDto.ProductId, itemDto.ProductName, itemDto.UnitPrice,
            itemDto.Quantity, itemDto.ImageUrl, basket);

        _basketRepo.AddBasketItem(basketItem);
        return Result.Success(basket);
    }

    public Result<BuyerBasket> ChangeItemQuantity(
        string buyerId, string productId, uint quantity)
    {
        if (quantity < 0)
            return Result.Invalid(new()
            {
                new ValidationError()
                {
                    Identifier = nameof(quantity),
                    ErrorMessage = "Quantity can not be a negative number."
                }
            });

        var basket = GetBasketOrCreateIfNotExist(buyerId);

        var item = basket.Items.SingleOrDefault(i => i.ProductId == productId);
        if (item == null)
            return Result.NotFound("Product has not been added to basket.");

        item.ChangeQuantity(quantity);
        basket.RemoveNoQuantityItems();
        _basketRepo.Update(basket);

        return Result.Success(basket);
    }

    public Result Checkout(BasketCheckout checkout)
    {
        var basket = GetBasketOrCreateIfNotExist(checkout.BuyerId);
        if (!basket.Items.Any())
            return Result.Conflict("Basket has no items.");

        var checkoutEvent = new BuyerCheckoutAcceptedIntegrationEvent(
            checkout.City, checkout.Country, checkout.ZipCode, checkout.ShippingAddress,
            checkout.CardNumber, checkout.CardHolderName, checkout.CardExpiration, checkout.CardSecurityNumber,
            checkout.BuyerId, basket);

        _eventBus.Publish(checkoutEvent);

        basket.EmptyBasket();
        _basketRepo.Update(basket);
        return Result.Success();
    }
}
