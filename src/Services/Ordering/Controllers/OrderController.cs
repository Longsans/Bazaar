using Bazaar.BuildingBlocks.Transactions.Abstractions;
using Bazaar.Ordering.Infrastructure.Transactional;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEventBus _eventBus;
        private readonly OrderingTransactionClient _txnClient;
        private readonly IResourceManager<Order, int> _orderRm;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderRepository orderRepository,
            IEventBus eventBus,
            OrderingTransactionClient transactionClient,
            IResourceManager<Order, int> orderRm,
            ILogger<OrderController> logger)
        {
            _orderRepo = orderRepository;
            _eventBus = eventBus;
            _txnClient = transactionClient;
            _orderRm = orderRm;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostAsync([FromBody] OrderCreateCommand createOrderCommand)
        {
            try
            {
                foreach (var item in createOrderCommand.Items)
                {
                    _logger.LogWarning("--ORD_CTRL: executing stock retrieval.");
                    var availableStock = await _txnClient.RetrieveProductAvailableStock(item.ProductExternalId);
                    _logger.LogWarning($"--ORD_CTRL: retrieved avail stock: {availableStock}.");
                    if (availableStock == null)
                        return NotFound(new { error = $"Product {item.ProductExternalId} does not exist." });
                    if (availableStock < item.Quantity)
                        return Conflict(new { error = $"Product {item.ProductExternalId} does not have enough stock to satisfy order." });
                    await _txnClient.AdjustProductAvailableStock(item.ProductExternalId, (int)availableStock - item.Quantity);
                }

                Order createdOrder;
                if (_txnClient.TransactionRef == null)
                {
                    _logger.LogWarning("--ORD_CTRL: no transaction created, default to local transaction.");
                    createdOrder = _orderRepo.CreateProcessingPayment(new Order(createOrderCommand));
                    _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(createdOrder.Id));
                    return Ok(createdOrder);
                }
                var txnState = _orderRm.GetOrCreateTransactionState(_txnClient.TransactionRef);
                createdOrder = new Order(createOrderCommand)
                {
                    Id = _orderRepo.NextOrderId,
                    Status = OrderStatus.ProcessingPayment
                };
                txnState.PendingInserts.Add(createdOrder);
                await _txnClient.Commit();
                _logger.LogWarning("--ORD_CTRL: transaction committed.");
                return Ok(createdOrder);
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

        [HttpPut]
        public IActionResult PutAsync([FromBody] Order order)
        {
            _orderRepo.Update(order);
            return Ok();
        }
    }
}