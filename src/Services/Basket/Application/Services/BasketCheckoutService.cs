namespace Bazaar.Basket.Application.Services;

public class BasketCheckoutService : IBasketCheckoutService
{
    private readonly IBasketRepository _basketRepo;
    private readonly IValidator<BasketCheckout> _checkoutValidator;
    private readonly IEventBus _eventBus;

    public BasketCheckoutService(
        IBasketRepository basketRepo,
        IValidator<BasketCheckout> checkoutValidator,
        IEventBus eventBus)
    {
        _basketRepo = basketRepo;
        _checkoutValidator = checkoutValidator;
        _eventBus = eventBus;
    }

    public Result Checkout(BasketCheckout checkout)
    {
        var validationResult = _checkoutValidator.Validate(checkout);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.Errors.Select(
                x => new ValidationError
                {
                    Identifier = nameof(checkout),
                    ErrorMessage = x.ErrorMessage
                }).ToList());
        }

        var basket = _basketRepo.GetWithItemsByBuyerId(checkout.BuyerId)
            ?? _basketRepo.Create(new BuyerBasket(checkout.BuyerId));

        if (!basket.Items.Any())
            return Result.Conflict("Basket has no items.");

        var checkoutEvent = new BasketCheckoutAcceptedIntegrationEvent(
            checkout.BuyerId, checkout.ShippingAddress, checkout.City,
            checkout.Country, checkout.ZipCode, basket);

        _eventBus.Publish(checkoutEvent);

        basket.EmptyBasket();
        _basketRepo.Update(basket);
        return Result.Success();
    }
}
