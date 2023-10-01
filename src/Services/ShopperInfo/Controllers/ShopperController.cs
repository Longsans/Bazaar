namespace Bazaar.ShopperInfo.Controllers;

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

        return shopper is not null
            ? shopper
            : NotFound();
    }

    [HttpGet]
    public ActionResult<Shopper> GetByExternalId(string externalId)
    {
        var shopper = _shopperRepo.GetByExternalId(externalId);

        return shopper is not null
            ? shopper
            : NotFound();
    }

    [HttpPost]
    public ActionResult<Shopper> Register(ShopperWriteCommand registerCommand)
    {
        var created = _shopperRepo.Register(registerCommand.ToShopperInfo());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdatedInfo(string externalId, ShopperWriteCommand updateCommand)
    {
        var update = updateCommand.ToShopper(externalId);

        return _shopperRepo.UpdateInfoByExternalId(update)
            ? Ok()
            : NotFound(new { externalId });
    }

    [HttpDelete("{externalId}")]
    public IActionResult Delete(string externalId)
    {
        return _shopperRepo.DeleteByExternalId(externalId)
            ? Ok()
            : NotFound(new { externalId });
    }
}
