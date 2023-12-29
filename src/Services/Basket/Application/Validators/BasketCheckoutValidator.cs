namespace Bazaar.Basket.Application.Validators;

public class BasketCheckoutValidator : AbstractValidator<BasketCheckout>
{
    public BasketCheckoutValidator()
    {
        RuleFor(x => x.BuyerId).NotEmpty();
        RuleFor(x => x.ShippingAddress).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.ZipCode).NotEmpty();
    }
}
