using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Basket.Controllers
{
    [Route("api/buyer-baskets")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepo;

        public BasketController(IBasketRepository basketRepo)
        {
            _basketRepo = basketRepo;
        }

        [HttpGet("{buyerId}")]
        public ActionResult<BuyerBasketQuery> GetBasketByBuyerId(string buyerId)
        {
            var basket = _basketRepo.GetBasketOrCreateIfNotExist(buyerId);
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
            [FromRoute] string buyerId, [FromBody] BasketItemWriteCommand addItemCommand)
        {
            var addResult = _basketRepo.AddItemToBasket(buyerId,
                new BasketItem
                {
                    ProductId = addItemCommand.ProductId,
                    ProductName = addItemCommand.ProductName,
                    UnitPrice = addItemCommand.UnitPrice,
                    Quantity = addItemCommand.Quantity,
                    ImageUrl = addItemCommand.ImageUrl
                });

            return addResult switch
            {
                BasketItemAlreadyAddedError => Conflict(new { error = "This product has already been added to this basket." }),
                BasketSuccessResult r => new BuyerBasketQuery(r.Basket),
            };
        }

        [HttpPut("{buyerId}/items/{productId}")]
        public ActionResult<BasketItemQuery> ChangeItemQuantity(
            string buyerId, string productId, [FromBody] uint quantity)
        {
            var result = _basketRepo.ChangeItemQuantity(buyerId, productId, quantity);
            return result switch
            {
                BasketItemSuccessResult r => new BasketItemQuery(r.BasketItem),
                QuantityLessThanOneErrorResult => BadRequest(new { error = "Quantity must be at least 1." }),
                BasketItemNotFoundErrorResult => NotFound(new { productId }),
                ExceptionErrorResult ex => StatusCode(500, new { error = ex.Error }),
            };
        }

        [HttpDelete("{buyerId}/items/{productId}")]
        public ActionResult<BuyerBasketQuery> RemoveItemFromBasket(string buyerId, string productId)
        {
            var result = _basketRepo.RemoveItemFromBasket(buyerId, productId);
            return result switch
            {
                BasketSuccessResult r => new BuyerBasketQuery(r.Basket),
                BasketItemNotFoundErrorResult => NotFound(new { productId }),
                ExceptionErrorResult ex => StatusCode(500, new { error = ex.Error }),
            };
        }
    }
}