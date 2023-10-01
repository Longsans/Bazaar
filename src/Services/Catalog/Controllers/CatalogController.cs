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
            var item = _catalogRepo.GetById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet]
        //[Authorize(Policy = "HasReadScope")]
        public ActionResult<IEnumerable<CatalogItem>> GetByCriteria(
            string? productId = null, string? sellerId = null, string? nameSubstring = null)
        {
            if (string.IsNullOrWhiteSpace(productId) &&
                string.IsNullOrWhiteSpace(sellerId) &&
                string.IsNullOrWhiteSpace(nameSubstring))
            {
                return BadRequest("At least one of the following arguments must be specified: " +
                    "productId, sellerId, partOfName");
            }
            if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(sellerId))
            {
                return BadRequest("Only one of the following arguments can be specified: productId, sellerId.");
            }
            if (!string.IsNullOrWhiteSpace(productId) && !string.IsNullOrWhiteSpace(nameSubstring))
            {
                return BadRequest("The productId and partOfName arguments cannot be combined.");
            }

            var items = new List<CatalogItem>();
            if (!string.IsNullOrWhiteSpace(productId))
            {
                var error = AddByProductIds(items, productId);
                if (error.HasValue)
                {
                    return NotFound(new { error.Value.productId, error.Value.error });
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(sellerId))
                {
                    AddOrFilterBySellerIds(ref items, sellerId!);
                }
                if (!string.IsNullOrWhiteSpace(nameSubstring))
                {
                    AddOrFilterByNameSubstring(ref items, nameSubstring!);
                }
            }
            return items;
        }

        [HttpPost]
        //[Authorize(Policy = "HasModifyScope")]
        public ActionResult<CatalogItem> Create(CatalogItemCreateCommand command)
        {
            var createdItem = _catalogRepo.Create(command.ToCatalogItem());
            return CreatedAtAction(nameof(GetById), new { createdItem.Id }, createdItem);
        }

        [HttpPut("{productId}")]
        //[Authorize(Policy = "HasModifyScope")]
        public IActionResult Update(string productId, CatalogItemUpdateCommand command)
        {
            var update = command.ToCatalogItem(productId);

            if (!_catalogRepo.Update(update))
                return NotFound(productId);

            return NoContent();
        }

        private (string productId, string error)? AddByProductIds(List<CatalogItem> items, string joinedProductIds)
        {
            var productIds = joinedProductIds.Split(',');

            foreach (var id in productIds)
            {
                var item = _catalogRepo.GetByProductId(id);
                if (item == null)
                {
                    return new(id, "Product not found.");
                }
                items.Add(item);
            }
            return null;
        }

        private void AddOrFilterBySellerIds(ref List<CatalogItem> items, string joinedSellerIds)
        {
            var sellerIds = joinedSellerIds.Split(',');

            if (items.Any())
            {
                items = items.Where(item => sellerIds.Contains(item.SellerId)).ToList();
            }

            foreach (var id in sellerIds)
            {
                items.AddRange(_catalogRepo.GetBySellerId(id));
            }
        }

        private void AddOrFilterByNameSubstring(ref List<CatalogItem> items, string nameSubstring)
        {
            if (items.Count == 0)
            {
                items.AddRange(_catalogRepo.GetByNameSubstring(nameSubstring));
            }
            else
            {
                items = items.Where(item => item.Name.Contains(nameSubstring)).ToList();
            }
        }
    }
}