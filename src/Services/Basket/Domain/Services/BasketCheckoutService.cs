namespace Bazaar.Basket.Domain.Services;

public class BasketCheckoutService : IBasketCheckoutService
{
    private readonly IBasketRepository _basketRepo;
    private readonly IEventBus _eventBus;

    public BasketCheckoutService(IBasketRepository basketRepo, IEventBus eventBus)
    {
        _basketRepo = basketRepo;
        _eventBus = eventBus;
    }

    public Result Checkout(BasketCheckout checkout)
    {
        var basket = _basketRepo.GetWithItemsByBuyerId(checkout.BuyerId)
            ?? _basketRepo.Create(new BuyerBasket(checkout.BuyerId));

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
