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
            [FromRoute] string buyerId, [FromBody] BasketItemWriteCommand itemWc)
        {
            var basket = _basketRepo.AddItemToBasket(buyerId,
                new BasketItem
                {
                    ProductId = itemWc.ProductId,
                    ProductName = itemWc.ProductName,
                    UnitPrice = itemWc.UnitPrice,
                    Quantity = itemWc.Quantity,
                    ImageUrl = itemWc.ImageUrl
                });
            if (basket == null)
            {
                return NotFound(buyerId);
            }

            var addedItem = basket.Items.FirstOrDefault(i => i.ProductId == itemWc.ProductId);
            if (addedItem == null)
            {
                return Conflict(itemWc.ProductId);
            }

            return CreatedAtAction(
                nameof(GetBasketItemByBuyerIdAndProductId),
                new { buyerId, productId = addedItem.ProductId },
                new BasketItemQuery(addedItem));
        }

        [HttpDelete("{buyerId}/items/{productId}")]
        public ActionResult<BuyerBasketQuery> RemoveItemFromBasket(string buyerId, string productId)
        {
            var basket = _basketRepo.RemoveItemFromBasket(buyerId, productId);
            if (basket == null)
            {
                return NotFound(new { buyerId, productId });
            }

            return new BuyerBasketQuery(basket);
        }
    }
}