using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly BasketManager _basketMgr;

    public BasketController(BasketManager basketManager)
    {
        _basketMgr = basketManager;
    }

    [HttpGet("{buyerId}")]
    public async Task<ActionResult<Basket>> GetByBuyerId(string buyerId)
    {
        var callResult = await _basketMgr.GetBasketByBuyerId(buyerId);
        if (callResult.IsUnauthorized)
        {
            return Unauthorized();
        }
        if (!callResult.IsSuccess)
        {
            return StatusCode(500, new { callResult.ErrorMessage });
        }
        return callResult.Result!;
    }

    [HttpPost("{buyerId}/items")]
    public async Task<ActionResult<Basket>> AddItemToBasket(string buyerId, BasketItemAddCommand command)
    {
        var basketItem = new BasketItem
        {
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
        var callResult = await _basketMgr.AddItemToBasket(buyerId, basketItem);

        if (callResult.IsSuccess)
            return callResult.Result!;

        return callResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.NotFound => NotFound(new { error = callResult.ErrorMessage }),
            ServiceCallError.BadRequest => BadRequest(new { error = callResult.ErrorMessage }),
            ServiceCallError.Conflict => Conflict(new { error = callResult.ErrorMessage }),
            _ => StatusCode(500, new { error = callResult.ErrorMessage })
        };
    }

    [HttpPatch("{buyerId}/items/{productId}")]
    public async Task<IActionResult> ChangeItemQuantity(
        string buyerId, string productId, [FromBody] uint quantity)
    {
        if (quantity == 0)
        {
            return BadRequest("Quantity must be greater than 0.");
        }

        var callResult = await _basketMgr.ChangeItemQuantity(buyerId, productId, quantity);

        if (callResult.IsSuccess)
            return NoContent();

        return callResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.NotFound => NotFound(new { error = callResult.ErrorMessage }),
            ServiceCallError.BadRequest => BadRequest(new { error = callResult.ErrorMessage }),
            _ => StatusCode(500, new { error = callResult.ErrorMessage }),
        };
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(BasketCheckout checkout)
    {
        var callResult = await _basketMgr.Checkout(checkout);

        if (callResult.IsSuccess)
            return Accepted();

        return callResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.BadRequest => BadRequest(new { error = callResult.ErrorMessage }),
            _ => StatusCode(500, new { error = callResult.ErrorMessage }),
        };
    }
}
