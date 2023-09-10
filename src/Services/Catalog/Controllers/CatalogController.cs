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

        [HttpGet("all")]
        [Authorize(Policy = "HasReadScope")]
        public ActionResult<IEnumerable<CatalogItem>> GetAll()
        {
            return _catalogRepo.GetItems().ToList();
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
        public ActionResult<CatalogItem> GetByProductId([FromQuery] string productId)
        {
            var item = _catalogRepo.GetItemByProductId(productId);
            if (item == null)
            {
                return NotFound();
            }
            return item;
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