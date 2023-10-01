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

        return callResult.IsSuccess
            ? callResult.Result!
            : callResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                _ => StatusCode(500, new { callResult.ErrorDetail })
            };
    }

    [HttpPost("{buyerId}/items")]
    public async Task<ActionResult<Basket>> AddItemToBasket(
        string buyerId, BasketItemAddCommand command)
    {
        var basketItem = new BasketItem
        {
            ProductId = command.ProductId,
            Quantity = command.Quantity
        };
        var addResult = await _basketMgr.AddItemToBasket(buyerId, basketItem);

        return addResult.IsSuccess
            ? addResult.Result!
            : addResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(new { error = addResult.ErrorDetail }),
                ServiceCallError.BadRequest => BadRequest(new { error = addResult.ErrorDetail }),
                ServiceCallError.Conflict => Conflict(new { error = addResult.ErrorDetail }),
                _ => StatusCode(500, new { error = addResult.ErrorDetail })
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

        var changeItemResult = await _basketMgr.ChangeItemQuantity(buyerId, productId, quantity);

        return changeItemResult.IsSuccess
            ? NoContent()
            : changeItemResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(new { error = changeItemResult.ErrorDetail }),
                ServiceCallError.BadRequest => BadRequest(new { error = changeItemResult.ErrorDetail }),
                _ => StatusCode(500, new { error = changeItemResult.ErrorDetail }),
            };
    }

    [HttpDelete("{buyerId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromBasket(
        string buyerId, string productId)
    {
        var removeResult = await _basketMgr.RemoveItemFromBasket(buyerId, productId);

        return removeResult.IsSuccess
            ? NoContent()
            : removeResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(removeResult.ErrorDetail),
                _ => StatusCode(500, new { removeResult.ErrorDetail })
            };
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(BasketCheckout checkout)
    {
        var checkoutResult = await _basketMgr.Checkout(checkout);

        return checkoutResult.IsSuccess
            ? Accepted()
            : checkoutResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.BadRequest => BadRequest(new { error = checkoutResult.ErrorDetail }),
                _ => StatusCode(500, new { error = checkoutResult.ErrorDetail }),
            };
    }
}
