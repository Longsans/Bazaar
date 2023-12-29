namespace Bazaar.Basket.Web.Validators;

public class BasketItemRequestValidator : AbstractValidator<AddBasketItemRequest>
{
    public BasketItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID cannot be empty.");
        RuleFor(x => x.ProductName).NotEmpty().WithMessage("Product name cannot be empty.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit price must be greater than 0.");
        RuleFor(x => x.Quantity).GreaterThan(0u).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.ImageUrl).NotEmpty().WithMessage("Image URL cannot be empty.");
    }
}
