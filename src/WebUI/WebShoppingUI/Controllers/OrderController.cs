using Microsoft.AspNetCore.Mvc;

namespace WebShoppingUI.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderManager _orderMgr;

    public OrderController(OrderManager orderMgr)
    {
        _orderMgr = orderMgr;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetByBuyerId(string buyerId)
    {
        var getResult = await _orderMgr.GetByBuyerIdAsync(buyerId);

        if (getResult.IsSuccess)
            return getResult.Result!.ToList();

        return getResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.BadRequest => BadRequest(getResult.ErrorDetail),
            _ => StatusCode(500, getResult.ErrorDetail)
        };
    }

    [HttpPatch("{orderId}")]
    public async Task<ActionResult<Order>> CancelOrder(int orderId, OrderCancellation cancellation)
    {
        var cancelResult = await _orderMgr.CancelOrder(orderId, cancellation.Reason);

        if (cancelResult.IsSuccess)
            return cancelResult.Result!;

        return cancelResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.NotFound => NotFound(cancelResult.ErrorDetail),
            ServiceCallError.Conflict => Conflict(cancelResult.ErrorDetail),
            _ => StatusCode(500, cancelResult.ErrorDetail)
        };
    }
}
