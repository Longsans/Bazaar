namespace WebShoppingUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketsController : ControllerBase
{
    private readonly BasketManager _basketMgr;

    public BasketsController(BasketManager basketManager)
    {
        _basketMgr = basketManager;
    }

    [HttpGet("{buyerId}")]
    public async Task<ActionResult<Basket>> GetByBuyerId(string buyerId)
    {
        var callResult = await _basketMgr.GetBasketByBuyerId(buyerId);

        return callResult.IsSuccess
            ? callResult.Value!
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
            ? addResult.Value!
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
        var changeItemResult = await _basketMgr.ChangeItemQuantity(buyerId, productId, quantity);
        return changeItemResult.ToActionResult();
    }

    [HttpPost("/api/checkouts")]
    public async Task<IActionResult> Checkout(BasketCheckout checkout)
    {
        var checkoutResult = await _basketMgr.Checkout(checkout);
        return checkoutResult.ToActionResult(Accepted());
    }
}
