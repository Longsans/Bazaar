namespace WebShoppingUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrderManager _orderMgr;

    public OrdersController(OrderManager orderMgr)
    {
        _orderMgr = orderMgr;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetByBuyerId(string buyerId,
        [FromQuery(Name = "productId")] string? productIds = null)
    {
        var getResult = !string.IsNullOrWhiteSpace(productIds)
            ? await _orderMgr.GetByBuyerIdContainingProductsAsync(buyerId, productIds.Split(','))
            : await _orderMgr.GetByBuyerIdAsync(buyerId);
        return getResult.ToActionResult();
    }

    [HttpPatch("{orderId}")]
    public async Task<ActionResult> CancelOrder(int orderId, OrderCancellation cancellation)
    {
        var cancelResult = await _orderMgr.CancelOrder(orderId, cancellation.Reason);
        return cancelResult.ToActionResult();
    }
}
