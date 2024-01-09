namespace WebShoppingUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShoppersController : ControllerBase
{
    private readonly IShopperInfoDataService _shopperService;

    public ShoppersController(IShopperInfoDataService shopperService)
    {
        _shopperService = shopperService;
    }

    [HttpGet("{shopperId}")]
    public async Task<ActionResult<ShopperQuery>> GetById(string shopperId)
    {
        var getResult = await _shopperService.GetByExternalId(shopperId);

        return getResult.IsSuccess
            ? new ShopperQuery(getResult.Value!)
            : getResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                _ => StatusCode(500, getResult.ErrorDetail)
            };
    }

    [HttpPatch("{shopperId}/personal-info")]
    public async Task<IActionResult> UpdateInfo(string shopperId, ShopperPersonalInfo request)
    {
        var updateResult = await _shopperService.UpdateInfo(shopperId, request);
        return updateResult.ToActionResult();
    }

    [HttpPatch("{shopperId}/email-address")]
    public async Task<IActionResult> ChangeEmailAddress(string shopperId, [FromBody] string emailAddress)
    {
        var updateResult = await _shopperService.ChangeEmailAddress(shopperId, emailAddress);
        return updateResult.ToActionResult();
    }

    [HttpPost]
    public async Task<ActionResult<Shopper>> Register(ShopperRegistration registration)
    {
        var registeredShopper = await _shopperService.Register(registration);
        return registeredShopper.ToActionResult();
    }
}
