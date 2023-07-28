namespace Bazaar.ShopperInfo.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class ShopperController : ControllerBase
    {
        private readonly IShopperRepository _shopperRepo;

        public ShopperController(IShopperRepository shopperRepo)
        {
            _shopperRepo = shopperRepo;
        }

        [HttpGet("{id}")]
        public ActionResult<Shopper> GetById(int id, [FromQuery] string? externalId = null)
        {
            Shopper? shopper;
            if (externalId != null)
            {
                shopper = _shopperRepo.GetByExternalId(externalId);
            }
            else
            {
                shopper = _shopperRepo.GetById(id);
            }

            if (shopper == null)
            {
                return NotFound();
            }
            return shopper;
        }

        [HttpPost]
        public ActionResult<Shopper> Create(Shopper shopper)
        {
            var created = _shopperRepo.Create(shopper);
            return CreatedAtAction(nameof(GetById), new { created.Id, created.ExternalId }, created);
        }

        [HttpPut]
        public IActionResult UpdatedInfo(Shopper shopper)
        {
            if (_shopperRepo.Update(shopper))
            {
                return Ok();
            }
            return NotFound(shopper.Id);
        }

        [HttpDelete("id")]
        public IActionResult Delete(int id)
        {
            if (_shopperRepo.Delete(id))
            {
                return Ok();
            }
            return NotFound(id);
        }
    }
}
