using Microsoft.AspNetCore.Mvc;

namespace WebSellerUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogDataService _catalogService;

        public CatalogController(ICatalogDataService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogItem>>> GetBySellerId(string sellerId)
        {
            var catalogResult = await _catalogService.GetBySellerId(sellerId);

            return catalogResult.IsSuccess
                ? catalogResult.Result!.ToList()
                : catalogResult.ErrorType switch
                {
                    ServiceCallError.Unauthorized => Unauthorized(),
                    ServiceCallError.BadRequest => BadRequest(catalogResult.ErrorDetail),
                    _ => StatusCode(500, catalogResult.ErrorDetail)
                };
        }

        [HttpPost]
        public async Task<IActionResult> Create(CatalogItemCreateCommand command)
        {
            var createResult = await _catalogService.Create(command);

            return createResult.IsSuccess
                ? CreatedAtAction(nameof(GetBySellerId), createResult.Result)
                : createResult.ErrorType switch
                {
                    ServiceCallError.Unauthorized => Unauthorized(),
                    _ => StatusCode(500, createResult.ErrorDetail)
                };
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(string productId, CatalogItemUpdateCommand update)
        {
            var updateResult = await _catalogService.Update(productId, update);

            return updateResult.IsSuccess
                ? NoContent()
                : updateResult.ErrorType switch
                {
                    ServiceCallError.Unauthorized => Unauthorized(),
                    ServiceCallError.NotFound => NotFound(),
                    _ => StatusCode(500, updateResult.ErrorDetail)
                };
        }
    }
}
