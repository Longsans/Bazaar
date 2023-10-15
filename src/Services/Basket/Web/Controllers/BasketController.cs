
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Basket.Web.Controllers
{
    [Route("api/buyer-baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketUseCases _basketUseCases;

        public BasketController(IBasketUseCases basketUseCases)
        {
            _basketUseCases = basketUseCases;
        }

        [HttpGet("{buyerId}")]
        public ActionResult<BuyerBasketQuery> GetBasketByBuyerId(string buyerId)
        {
            var basket = _basketUseCases.GetBasketOrCreateIfNotExist(buyerId);
            return new BuyerBasketQuery(basket);
        }

        [HttpGet("{buyerId}/items/{productId}")]
        public ActionResult<BasketItemQuery> GetBasketItemByBuyerIdAndProductId(
            [FromRoute] string buyerId, [FromRoute] string productId)
        {
            var basketItem = _basketUseCases.GetBasketItem(buyerId, productId);
            if (basketItem == null)
                return NotFound(new { buyerId, productId });

            return new BasketItemQuery(basketItem);
        }

        [HttpPost("{buyerId}/items")]
        public ActionResult<BuyerBasketQuery> AddItemToBasket(
            [FromRoute] string buyerId, [FromBody] AddBasketItemRequest request)
        {
            var basketItemDto = new BasketItemDto
            {
                ProductId = request.ProductId,
                ProductName = request.ProductName,
                UnitPrice = request.UnitPrice,
                Quantity = request.Quantity,
                ImageUrl = request.ImageUrl,
            };

            var addResult = _basketUseCases.AddItemToBasket(buyerId, basketItemDto);

            return addResult.Map(b => new BuyerBasketQuery(b))
                .ToActionResult(this);
        }

        [HttpPatch("{buyerId}/items/{productId}")]
        public ActionResult<BuyerBasketQuery> ChangeItemQuantity(
            string buyerId, string productId, [FromBody] uint quantity)
        {
            var changeItemResult = _basketUseCases.ChangeItemQuantity(buyerId, productId, quantity);

            return changeItemResult.Map(b => new BuyerBasketQuery(b))
                .ToActionResult(this);
        }

        [HttpPost("/api/checkout")]
        public IActionResult Checkout(BasketCheckout checkout)
        {
            var checkoutResult = _basketUseCases.Checkout(checkout);
            return checkoutResult.ToActionResult(this);
        }
    }
}