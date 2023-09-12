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
        public ActionResult<Shopper> GetById(int id)
        {
            var shopper = _shopperRepo.GetById(id);
            if (shopper == null)
            {
                return NotFound();
            }

            return shopper;
        }

        [HttpGet]
        public ActionResult<Shopper> GetByExternalId([FromQuery] string externalId)
        {
            var shopper = _shopperRepo.GetByExternalId(externalId);
            if (shopper == null)
            {
                return NotFound();
            }

            return shopper;
        }

        [HttpPost]
        public ActionResult<Shopper> Register(ShopperWriteCommand registerCommand)
        {
            var created = _shopperRepo.Register(registerCommand.ToShopperInfo());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatedInfo(int id, ShopperWriteCommand updateCommand)
        {
            var update = updateCommand.ToShopperInfo();
            update.Id = id;

            if (!_shopperRepo.UpdateInfo(update))
            {
                return NotFound(id);
            }

            return Ok();
        }

        [HttpDelete("id")]
        public IActionResult Delete(int id)
        {
            if (!_shopperRepo.Delete(id))
            {
                return NotFound(id);
            }

            return Ok();
        }
    }
}
