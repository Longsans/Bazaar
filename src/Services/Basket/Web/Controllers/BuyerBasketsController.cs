namespace Bazaar.Basket.Web.Controllers;

[Route("api/buyer-baskets")]
[ApiController]
public class BuyerBasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepo;
    private readonly IBasketCheckoutService _checkoutService;
    private readonly IValidator<AddBasketItemRequest> _basketItemReqValidator;

    public BuyerBasketsController(IBasketRepository basketRepository,
        IBasketCheckoutService checkoutService,
        IValidator<AddBasketItemRequest> basketItemReqValidator)
    {
        _basketRepo = basketRepository;
        _checkoutService = checkoutService;
        _basketItemReqValidator = basketItemReqValidator;
    }

    [HttpGet("{buyerId}")]
    public ActionResult<BuyerBasketQuery> GetBasketByBuyerId(string buyerId)
    {
        var basket = GetBasketOrCreateIfNotExist(buyerId);
        return new BuyerBasketQuery(basket);
    }

    [HttpGet("{buyerId}/items/{productId}")]
    public ActionResult<BasketItemQuery> GetBasketItemByBuyerIdAndProductId(
        [FromRoute] string buyerId, [FromRoute] string productId)
    {
        var basketItem = _basketRepo.GetBasketItem(buyerId, productId);
        if (basketItem == null)
        {
            return NotFound(new { buyerId, productId });
        }

        return new BasketItemQuery(basketItem);
    }

    [HttpPost("{buyerId}/items")]
    public ActionResult<BuyerBasketQuery> AddItemToBasket(
        [FromRoute] string buyerId, [FromBody] AddBasketItemRequest request)
    {
        var validationResult = _basketItemReqValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(
                new
                {
                    errors = validationResult.Errors.Select(x => x.ErrorMessage)
                });
        }

        var basket = GetBasketOrCreateIfNotExist(buyerId);
        var basketItem = new BasketItem(
                request.ProductId, request.ProductName, request.UnitPrice,
                request.Quantity, request.ImageUrl, basket);

        if (!basket.AddItem(basketItem).IsSuccess)
        {
            return Conflict(new { error = "Basket already contains this item." });
        }
        _basketRepo.Update(basket);
        return new BuyerBasketQuery(basket);
    }

    [HttpPatch("{buyerId}/items/{productId}")]
    public ActionResult<BuyerBasketQuery> ChangeItemQuantity(
        string buyerId, string productId, [FromBody] uint quantity)
    {
        var basket = GetBasketOrCreateIfNotExist(buyerId);

        if (!basket.ChangeItemQuantity(productId, quantity).IsSuccess)
        {
            return Conflict(new { error = "Basket does not contain this product." });
        }
        _basketRepo.Update(basket);

        return new BuyerBasketQuery(basket);
    }

    [HttpPost("/api/checkouts")]
    public IActionResult Checkout(BasketCheckout checkout)
    {
        var checkoutResult = _checkoutService.Checkout(checkout);
        return checkoutResult.ToActionResult(this);
    }

    private BuyerBasket GetBasketOrCreateIfNotExist(string buyerId)
    {
        var basket = _basketRepo.GetWithItemsByBuyerId(buyerId)
            ?? _basketRepo.Create(new BuyerBasket(buyerId));

        return basket;
    }
}