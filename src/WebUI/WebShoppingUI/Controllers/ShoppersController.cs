using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppersController : ControllerBase
    {
        private readonly ShopperInfoService _shopperService;

        public ShoppersController(ShopperInfoService shopperService)
        {
            _shopperService = shopperService;
        }

        [HttpGet("{shopperId}")]
        public async Task<ActionResult<ShopperQuery>> GetById(string shopperId)
        {
            var shopper = await _shopperService.GetByExternalId(shopperId);
            if (shopper == null)
            {
                return Unauthorized();
            }
            return new ShopperQuery(shopper);
        }
    }
}
