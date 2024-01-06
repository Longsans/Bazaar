namespace Bazaar.ShopperInfo.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppersController : ControllerBase
{
    private readonly IShopperRepository _shopperRepo;

    public ShoppersController(IShopperRepository shopperRepo)
    {
        _shopperRepo = shopperRepo;
    }

    [HttpGet("{id}")]
    public ActionResult<Shopper> GetById(int id)
    {
        var shopper = _shopperRepo.GetById(id);

        return shopper != null
            ? shopper
            : NotFound();
    }

    [HttpGet]
    public ActionResult<Shopper> GetByExternalId(string externalId)
    {
        var shopper = _shopperRepo.GetByExternalId(externalId);

        return shopper != null
            ? shopper
            : NotFound();
    }

    [HttpPost]
    public ActionResult<Shopper> Register(ShopperWriteRequest request)
    {
        var created = _shopperRepo.Create(request.ToNewShopper());
        return CreatedAtAction(
            nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdatedInfo(string externalId, ShopperWriteRequest request)
    {
        var update = request.ToExistingShopper(externalId);
        var existingShopper = _shopperRepo.GetByExternalId(externalId);

        if (existingShopper == null)
            return NotFound(new { externalId });

        _shopperRepo.Update(update);
        return NoContent();
    }
}
