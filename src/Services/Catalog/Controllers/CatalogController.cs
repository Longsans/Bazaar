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
        public IActionResult GetById(int id)
        {
            var item = _catalogRepo.GetItemById(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet]
        public ActionResult<IEnumerable<CatalogItem>> GetManyByProductId([FromQuery] IEnumerable<string> productIds)
        {
            return _catalogRepo.GetManyByProductId(productIds).ToList();
        }

        [HttpPost]
        public IActionResult Create(CatalogItem item)
        {
            var createdItem = _catalogRepo.Create(item);
            return CreatedAtAction(nameof(GetById), createdItem.Id, createdItem);
        }

        [HttpPut]
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