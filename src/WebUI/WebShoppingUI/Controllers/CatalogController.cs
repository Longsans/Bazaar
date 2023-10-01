using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{
    private readonly ICatalogDataService _catalogService;

    public CatalogController(ICatalogDataService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItem>>> GetByPartOfName(string partOfName)
    {
        var callResult = await _catalogService.GetByNameSubstring(partOfName);
        if (callResult.IsUnauthorized)
        {
            return Unauthorized();
        }
        if (!callResult.IsSuccess)
        {
            return StatusCode(500, new { error = callResult.ErrorDetail });
        }
        return callResult.Result!.ToList();
    }
}
