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
        try
        {
            var created = _shopperRepo.Create(request.ToNewShopper());
            return CreatedAtAction(
                nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{externalId}")]
    public IActionResult UpdatedInfo(string externalId, ShopperWriteRequest request)
    {
        var shopper = _shopperRepo.GetByExternalId(externalId);
        if (shopper == null)
            return NotFound(new { externalId });

        try
        {
            shopper.UpdatePersonalInfo(
                request.FirstName, request.LastName, request.EmailAddress,
                request.PhoneNumber, request.DateOfBirth, request.Gender);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        _shopperRepo.Update(shopper);
        return NoContent();
    }
}
