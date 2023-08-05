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
            var basket = _basketRepo.GetByBuyerId(buyerId);
            if (basket == null)
                return NotFound();
            return new BuyerBasketQuery(basket);
        }

        [HttpGet("{buyerId}/items/{productId}")]
        public ActionResult<BasketItemQuery> GetBasketItemByBuyerIdAndProductId(
            [FromRoute] string buyerId, [FromRoute] string productId)
        {
            var basket = _basketRepo.GetByBuyerId(buyerId);
            if (basket == null)
            {
                return NotFound();
            }

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                return NotFound();
            }
            return new BasketItemQuery(item);
        }

        [HttpPost("{buyerId}/items")]
        public ActionResult<BasketItemQuery> AddItemToBasket(
            [FromRoute] string buyerId, [FromBody] BasketItemWriteCommand addItemCommand)
        {
            var basket = _basketRepo.AddItemToBasket(buyerId,
                new BasketItem
                {
                    ProductId = addItemCommand.ProductId,
                    ProductName = addItemCommand.ProductName,
                    UnitPrice = addItemCommand.UnitPrice,
                    Quantity = addItemCommand.Quantity,
                    ImageUrl = addItemCommand.ImageUrl
                });

            if (basket == null)
            {
                return NotFound(buyerId);
            }

            var addedItem = basket.Items.FirstOrDefault(i => i.ProductId == addItemCommand.ProductId);
            if (addedItem == null)
            {
                return Conflict(addItemCommand.ProductId);
            }

            return CreatedAtAction(
                nameof(GetBasketItemByBuyerIdAndProductId),
                new { buyerId, productId = addedItem.ProductId },
                new BasketItemQuery(addedItem));
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
                BasketNotFoundErrorResult => NotFound(new { buyerId }),
                BasketItemNotFoundErrorResult => NotFound(new { productId }),
                ExceptionErrorResult ex => StatusCode(500, new { error = ex.Error }),
                _ => StatusCode(500),
            };
        }

        [HttpDelete("{buyerId}/items/{productId}")]
        public ActionResult<BuyerBasketQuery> RemoveItemFromBasket(string buyerId, string productId)
        {
            var result = _basketRepo.RemoveItemFromBasket(buyerId, productId);
            return result switch
            {
                BasketSuccessResult r => new BuyerBasketQuery(r.Basket),
                BasketNotFoundErrorResult => NotFound(new { buyerId }),
                BasketItemNotFoundErrorResult => NotFound(new { productId }),
                ExceptionErrorResult ex => StatusCode(500, new { error = ex.Error }),
                _ => StatusCode(500),
            };
        }
    }
}