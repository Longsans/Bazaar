using Microsoft.AspNetCore.Mvc;

namespace WebSellerUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogSvc;

        public CatalogController(CatalogService catalogService)
        {
            _catalogSvc = catalogService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatalogItem>>> Get([FromQuery] string sellerId)
        {
            IEnumerable<CatalogItem>? catalogItems;

            if (!string.IsNullOrWhiteSpace(sellerId))
            {
                catalogItems = await _catalogSvc.GetBySellerId(sellerId);
            }
            else
            {
                catalogItems = await _catalogSvc.GetAllItems();
                if (catalogItems == null)
                {
                    return Unauthorized();
                }
            }
            return Ok(catalogItems);
        }

        [HttpPut]
        public async Task<IActionResult> Update(CatalogItem update)
        {
            await _catalogSvc.Update(update);
            return NoContent();
        }
    }
}
