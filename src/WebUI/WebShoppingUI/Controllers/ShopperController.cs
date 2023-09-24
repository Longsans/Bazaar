using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class ShopperController : ControllerBase
{
    private readonly IShopperInfoDataService _shopperService;

    public ShopperController(IShopperInfoDataService shopperService)
    {
        _shopperService = shopperService;
    }

    [HttpGet("{shopperId}")]
    public async Task<ActionResult<ShopperQuery>> GetById(string shopperId)
    {
        var getResult = await _shopperService.GetByExternalId(shopperId);

        return getResult.IsSuccess
            ? new ShopperQuery(getResult.Result!)
            : getResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                _ => StatusCode(500, getResult.ErrorDetail)
            };
    }

    [HttpPut("{shopperId}")]
    public async Task<IActionResult> UpdateInfo(
        string shopperId, ShopperWriteCommand updateCommand)
    {
        var updateResult = await _shopperService.UpdateInfo(shopperId, updateCommand);

        return updateResult.IsSuccess
            ? NoContent()
            : updateResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                _ => StatusCode(500, updateResult.ErrorDetail)
            };
    }
}
