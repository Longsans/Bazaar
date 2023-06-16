using Microsoft.AspNetCore.Mvc;

namespace Bazaar.ApiGateways.WebBff.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly OrderingTransactionClient _txnClient;
        private readonly ILogger<OrderingController> _logger;

        public OrderingController(
            OrderingTransactionClient txnClient,
            ILogger<OrderingController> logger)
        {
            _txnClient = txnClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<OrderQuery>> PostAsync([FromBody] OrderCreateCommand createOrderCommand)
        {
            try
            {
                foreach (var item in createOrderCommand.Items)
                {
                    _logger.LogWarning("--ORDGW_CTRL: executing stock retrieval.");
                    var availableStock = await _txnClient.RetrieveProductAvailableStock(item.ProductExternalId);
                    _logger.LogWarning($"--ORDGW_CTRL: retrieved avail stock: {availableStock}.");
                    if (availableStock == null)
                        return NotFound(new { error = $"Product {item.ProductExternalId} does not exist." });
                    if (availableStock < item.Quantity)
                        return Conflict(new { error = $"Product {item.ProductExternalId} does not have enough stock to satisfy order." });
                    await _txnClient.AdjustProductAvailableStock(item.ProductExternalId, (int)availableStock - item.Quantity);
                }

                if (_txnClient.TransactionRef == null)
                {
                    _logger.LogWarning("--ORDGW_CTRL: retrieved no items info for order, creating new transaction.");
                    await _txnClient.BeginTransactionIfNotInOne();
                }

                var createdOrder = await _txnClient.CreateProcessingOrder(createOrderCommand);
                await _txnClient.Commit();
                _logger.LogWarning("--ORDGW_CTRL: transaction committed.");
                return createdOrder != null ?
                    Ok(createdOrder) : StatusCode(500, new { error = "Server exception: Transaction failed." });
            }
            catch (KeyNotFoundException e)
            {
                return Conflict(new { error = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
