namespace Bazaar.ApiGateways.WebBff.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class BasketController : ControllerBase
{
    [HttpPost("{basketId}/items/")]
    public IActionResult AddItemToBasket()
    {
        // check catalog item's quantity satifies quantity added to basket
        return Ok();
    }

    [HttpPut("{basketId}/items/{itemId}")]
    public IActionResult ChangeItemQuantity()
    {
        // also check item quantity satisfaction
        return Ok();
    }
}
