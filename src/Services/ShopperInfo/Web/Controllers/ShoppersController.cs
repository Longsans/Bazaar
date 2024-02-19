namespace Bazaar.ShopperInfo.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppersController : ControllerBase
{
    private readonly IShopperRepository _shopperRepo;
    private readonly ShopperEmailAddressService _emailAddressService;

    public ShoppersController(IShopperRepository shopperRepo, ShopperEmailAddressService emailAddressService)
    {
        _shopperRepo = shopperRepo;
        _emailAddressService = emailAddressService;
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
    public ActionResult<Shopper> Register(ShopperRegistration registration)
    {
        try
        {
            var created = _shopperRepo.Create(registration.ToNewShopper());
            return CreatedAtAction(
                nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPatch("{externalId}/personal-info")]
    public IActionResult UpdatePersonalInfo(string externalId, ShopperPersonalInfo request)
    {
        var shopper = _shopperRepo.GetByExternalId(externalId);
        if (shopper == null)
            return NotFound(new { externalId });

        try
        {
            shopper.UpdatePersonalInfo(request.FirstName, request.LastName,
                request.PhoneNumber, request.DateOfBirth, request.Gender);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        _shopperRepo.Update(shopper);
        return NoContent();
    }

    [HttpPatch("{externalId}/email-address")]
    public IActionResult ChangeEmailAddress(string externalId, [FromBody] string emailAddress)
    {
        try
        {
            _emailAddressService.ChangeEmailAddress(externalId, emailAddress);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (DuplicateEmailAddressException)
        {
            return Conflict(new { error = "Email address has already been taken." });
        }
        return Ok();
    }
}
