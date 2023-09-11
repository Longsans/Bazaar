using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepo;

        public CatalogController(ICatalogRepository catalogRepo)
        {
            _catalogRepo = catalogRepo;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "HasReadScope")]
        public IActionResult GetById(int id)
        {
            var item = _catalogRepo.GetItemById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet]
        [Authorize(Policy = "HasReadScope")]
        public ActionResult<IEnumerable<CatalogItem>> GetByProductIds(
            [FromQuery] string? productId = null, [FromQuery] string? sellerId = null)
        {
            if (string.IsNullOrWhiteSpace(productId) && string.IsNullOrWhiteSpace(sellerId) ||
                !string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(sellerId))
            {
                return BadRequest("Exactly one of the following parameters must be specified: productId, sellerId.");
            }

            var items = new List<CatalogItem>();
            if (string.IsNullOrWhiteSpace(productId))
            {
                var sellerIds = sellerId.Split(',');

                foreach (var id in sellerIds)
                {
                    items.AddRange(_catalogRepo.GetBySellerId(id));
                }
            }
            else
            {
                var productIds = productId.Split(',');

                foreach (var id in productIds)
                {
                    var item = _catalogRepo.GetItemByProductId(id);
                    if (item == null)
                    {
                        return NotFound(new { productId = id, error = "Product not found." });
                    }
                    items.Add(item);
                }
            }
            return items;
        }

        [HttpPost]
        [Authorize(Policy = "HasModifyScope")]
        public IActionResult Create(CatalogItem item)
        {
            var createdItem = _catalogRepo.Create(item);
            return CreatedAtAction(nameof(GetById), createdItem.Id, createdItem);
        }

        [HttpPut]
        [Authorize(Policy = "HasModifyScope")]
        public IActionResult Update(CatalogItem update)
        {
            if (!_catalogRepo.Update(update))
            {
                return NotFound(update.Id);
            }
            return NoContent();
        }
    }
}