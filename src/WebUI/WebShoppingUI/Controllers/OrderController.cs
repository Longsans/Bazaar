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
        var callResult = await _orderMgr.GetByBuyerIdAsync(buyerId);

        if (callResult.IsSuccess)
            return callResult.Result!.ToList();

        return callResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.BadRequest => BadRequest(callResult.ErrorDetail),
            _ => StatusCode(500, callResult.ErrorDetail)
        };
    }

    [HttpPatch("{orderId}")]
    public async Task<ActionResult<Order>> CancelOrder(int orderId)
    {
        var callResult = await _orderMgr.CancelOrder(orderId);

        if (callResult.IsSuccess)
            return callResult.Result!;

        return callResult.ErrorType switch
        {
            ServiceCallError.Unauthorized => Unauthorized(),
            ServiceCallError.NotFound => NotFound(callResult.ErrorDetail),
            ServiceCallError.Conflict => Conflict(callResult.ErrorDetail),
            _ => StatusCode(500, callResult.ErrorDetail)
        };
    }
}
