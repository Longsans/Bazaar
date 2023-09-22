using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ShopperController : ControllerBase
    {
        private readonly HttpShopperInfoService _shopperService;

        public ShopperController(HttpShopperInfoService shopperService)
        {
            _shopperService = shopperService;
        }

        [HttpGet("{shopperId}")]
        public async Task<ActionResult<ShopperQuery>> GetById(string shopperId)
        {
            var callResult = await _shopperService.GetByExternalId(shopperId);

            if (callResult.IsSuccess)
                return new ShopperQuery(callResult.Result!);

            return callResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                _ => StatusCode(500, callResult.ErrorMessage)
            };
        }
    }
}
