namespace WebSellerUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebSellerUI.DTOs;
using WebSellerUI.Managers;

[Route("api/orders")]
[ApiController]
public class OrderingController : ControllerBase
{
    private readonly OrderManager _orderMgr;

    public OrderingController(OrderManager orderManager)
    {
        _orderMgr = orderManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetBySellerId(
        [FromQuery(Name = "sellerId")] string sellerIds)
    {
        var callResult = await _orderMgr.GetBySellerId(sellerIds);

        return callResult.IsSuccess
            ? callResult.Result!.ToList()
            : callResult.ErrorType switch
            {
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.BadRequest => BadRequest(callResult.ErrorDetail),
                _ => StatusCode(500, callResult.ErrorDetail)
            };
    }

    [HttpPatch("{orderId}")]
    public async Task<IActionResult> ConfirmOrCancelOrder(int orderId, [FromBody] OrderConfirmation confirmation)
    {
        if (!confirmation.Confirmed && string.IsNullOrWhiteSpace(confirmation.CancelReason))
            return BadRequest("Order cancellation must include a reason from the seller.");

        var callResult = confirmation.Confirmed
            ? await _orderMgr.ConfirmOrder(orderId)
            : await _orderMgr.CancelOrder(orderId, confirmation.CancelReason!);

        return callResult.IsSuccess
            ? NoContent()
            : callResult.ErrorType switch
            {
                ServiceCallError.BadRequest => BadRequest(callResult.ErrorDetail),
                ServiceCallError.Unauthorized => Unauthorized(),
                ServiceCallError.NotFound => NotFound(),
                ServiceCallError.Conflict => Conflict(callResult.ErrorDetail),
                _ => StatusCode(500, callResult.ErrorDetail)
            };
    }
}
