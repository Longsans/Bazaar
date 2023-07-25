using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Basket.Controllers
{
    [Route("api/[controller]")]
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
    }
}