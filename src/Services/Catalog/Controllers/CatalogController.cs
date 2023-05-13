using Microsoft.AspNetCore.Mvc;
using Catalog.Model;

namespace Catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = new CatalogItem
            {
                Id = id,
                Name = "The Winds of Winter",
                Description = "Book 6 of ASOIAF",
                Price = 34.99m,
                AvailableStock = 32,
                RestockThreshold = 30,
                MaxStockThreshold = 1000,
            };
            return Ok(item);
        }
    }
}