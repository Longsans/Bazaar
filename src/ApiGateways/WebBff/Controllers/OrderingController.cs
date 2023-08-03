namespace Bazaar.ApiGateways.WebBff.Controllers
{
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly IOrderingTransactionClient _txnClient;
        private readonly ILogger<OrderingController> _logger;

        public OrderingController(
            IOrderingTransactionClient txnClient,
            ILogger<OrderingController> logger)
        {
            _txnClient = txnClient;
            _logger = logger;
        }

        [HttpPost("api/orders")]
        public async Task<ActionResult<OrderQuery>> CreateOrderAsync([FromBody] OrderCreateCommand createOrderCommand)
        {
            if (createOrderCommand.Items.Count == 0)
                return BadRequest(new { error = "Order has no items." });

            try
            {
                foreach (var item in createOrderCommand.Items)
                {
                    _logger.LogWarning("--ORDGW_CTRL: executing stock retrieval.");
                    var availableStock = await _txnClient.RetrieveProductAvailableStock(item.ProductId);
                    _logger.LogWarning($"--ORDGW_CTRL: retrieved available stock: {availableStock}.");

                    if (availableStock < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Product {item.ProductId} does not have enough stock to satisfy order.");
                    }
                    await _txnClient.AdjustProductAvailableStock(
                        item.ProductId, (int)availableStock - item.Quantity);
                }

                var createdOrder = await _txnClient.CreateProcessingOrder(createOrderCommand);
                await _txnClient.Commit();
                _logger.LogWarning("--ORDGW_CTRL: transaction commit executed.");
                return Ok(createdOrder);
            }
            catch (KeyNotFoundException e)
            {
                await _txnClient.Rollback();
                return NotFound(new { error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                await _txnClient.Rollback();
                return Conflict(new { error = e.Message });
            }
            catch (Exception e)
            {
                await _txnClient.Rollback();
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("api/orders-fail")]
        public async Task<ActionResult<OrderQuery>> CreateOrderAsyncFail(
            [FromBody] OrderCreateCommand createOrderCommand)
        {
            if (createOrderCommand.Items.Count == 0)
                return BadRequest(new { error = "Order has no items." });

            try
            {
                foreach (var item in createOrderCommand.Items)
                {
                    _logger.LogWarning("--ORDGW_CTRL: executing stock retrieval.");
                    var availableStock = await _txnClient.RetrieveProductAvailableStock(item.ProductId);
                    _logger.LogWarning($"--ORDGW_CTRL: retrieved available stock: {availableStock}.");

                    if (availableStock < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Product {item.ProductId} does not have enough stock to satisfy order.");
                    }
                    await _txnClient.AdjustProductAvailableStock(item.ProductId, (int)availableStock - item.Quantity);
                }

                throw new InvalidOperationException("This operation failed and catalog item's stock has been rolled-back.");
            }
            catch (KeyNotFoundException e)
            {
                await _txnClient.Rollback();
                return NotFound(new { error = e.Message });
            }
            catch (InvalidOperationException e)
            {
                await _txnClient.Rollback();
                return Conflict(new { error = e.Message });
            }
            catch (Exception e)
            {
                await _txnClient.Rollback();
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
