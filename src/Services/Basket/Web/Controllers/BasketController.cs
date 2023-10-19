
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Basket.Web.Controllers;

[Route("api/buyer-baskets")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _basketRepo;
    private readonly IBasketCheckoutService _checkoutService;

    public BasketController(IBasketRepository basketRepository, IBasketCheckoutService checkoutService)
    {
        _basketRepo = basketRepository;
        _checkoutService = checkoutService;
    }

    [HttpGet("{buyerId}")]
    public ActionResult<BuyerBasketQuery> GetBasketByBuyerId(string buyerId)
    {
        var basket = GetWithItemsOrCreateBasketIfNotExist(buyerId);
        return new BuyerBasketQuery(basket);
    }

    [HttpGet("{buyerId}/items/{productId}")]
    public ActionResult<BasketItemQuery> GetBasketItemByBuyerIdAndProductId(
        [FromRoute] string buyerId, [FromRoute] string productId)
    {
        var basketItem = _basketRepo.GetBasketItem(buyerId, productId);
        if (basketItem == null)
            return NotFound(new { buyerId, productId });

        return new BasketItemQuery(basketItem);
    }

    [HttpPost("{buyerId}/items")]
    public ActionResult<BuyerBasketQuery> AddItemToBasket(
        [FromRoute] string buyerId, [FromBody] AddBasketItemRequest request)
    {
        var basket = GetWithItemsOrCreateBasketIfNotExist(buyerId);

        try
        {
            var basketItem = new BasketItem(
                request.ProductId, request.ProductName, request.UnitPrice,
                request.Quantity, request.ImageUrl, basket);

            basket.AddItem(basketItem);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ProductAlreadyInBasketException ex)
        {
            return Conflict(new { error = ex.Message });
        }

        _basketRepo.Update(basket);
        return new BuyerBasketQuery(basket);
    }

    [HttpPatch("{buyerId}/items/{productId}")]
    public ActionResult<BuyerBasketQuery> ChangeItemQuantity(
        string buyerId, string productId, [FromBody] uint quantity)
    {
        var basket = GetWithItemsOrCreateBasketIfNotExist(buyerId);

        try
        {
            basket.ChangeItemQuantity(productId, quantity);
        }
        catch (ProductNotInBasketException ex)
        {
            return Conflict(new { error = ex.Message });
        }

        return new BuyerBasketQuery(basket);
    }

    [HttpPost("/api/checkout")]
    public IActionResult Checkout(BasketCheckout checkout)
    {
        var checkoutResult = _checkoutService.Checkout(checkout);
        return checkoutResult.ToActionResult(this);
    }

    private BuyerBasket GetWithItemsOrCreateBasketIfNotExist(string buyerId)
    {
        var basket = _basketRepo.GetWithItemsByBuyerId(buyerId)
            ?? _basketRepo.Create(new BuyerBasket(buyerId));

        return basket;
    }
}