using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Basket.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _basketRepo;

        public BasketController(IBasketRepository basketRepo)
        {
            _basketRepo = basketRepo;
        }

        [HttpGet("{buyerId}")]
        public IActionResult GetBasketByBuyerId(string buyerId)
        {
            var basket = _basketRepo.GetByBuyerId(buyerId);
            if (basket == null)
                return NotFound();
            return Ok(basket);
        }

        [HttpPost("{buyerId}")]
        public ActionResult<CustomerBasket> UpdateBasket(
            [FromRoute] string buyerId, [FromBody] CustomerBasket basket)
        {
            _basketRepo.Update(buyerId, basket);
            return basket;
        }
    }
}