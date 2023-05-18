using Microsoft.AspNetCore.Mvc;
using Bazaar.Catalog.Model;

namespace Bazaar.Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}