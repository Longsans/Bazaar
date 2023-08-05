namespace Bazaar.Ordering.Adapters.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEventBus _eventBus;
        private readonly IResourceManager<Order, int> _orderRm;

        public OrderController(
            IOrderRepository orderRepository,
            IEventBus eventBus,
            IResourceManager<Order, int> orderRm)
        {
            _orderRepo = orderRepository;
            _eventBus = eventBus;
            _orderRm = orderRm;
        }

        [HttpGet("{id}")]
        public ActionResult<OrderQuery> GetById(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
                return NotFound();
            return new OrderQuery(order);
        }

        [HttpGet]
        public ActionResult<IEnumerable<OrderQuery>> GetAll()
        {
            var orders = _orderRepo.GetAll().Select(o => new OrderQuery(o));
            return Ok(orders);
        }

        [HttpPost]
        public ActionResult<OrderQuery> CreateOrder(OrderWriteCommand command)
        {
            var order = _orderRepo.CreateProcessingPaymentOrder(command.ToOrder());
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, new OrderQuery(order));
        }

        [HttpPatch("{id}")]
        public ActionResult<OrderQuery> UpdateOrderStatus(int id, OrderStatus status)
        {
            var updateResult = _orderRepo.UpdateStatus(id, status);
            return updateResult switch
            {
                OrderSuccessResult r => new OrderQuery(r.Order),
                OrderNotFoundError => NotFound(new { id }),
                InvalidOrderCancellationError e => Conflict(new { error = e.Error }),
                _ => StatusCode(500)
            };
        }

        [HttpPost("/api/txn/{txn}/orders")]
        public ActionResult<OrderQuery> CreateOrderInTransaction(
            [FromRoute] TransactionRef txn, [FromBody] OrderWriteCommand command)
        {
            try
            {
                var txnState = _orderRm.GetOrCreateTransactionState(txn);
                var createdOrder = command.ToOrder();
                createdOrder.Status = OrderStatus.ProcessingPayment;
                txnState.PendingInserts.Add(createdOrder);
                return new OrderQuery(createdOrder);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("/api/txn")]
        public IActionResult PrepareToCommitTransaction([FromBody] TransactionRef txn)
        {
            _orderRm.HandlePrepare(txn);
            return Ok();
        }

        [HttpPut("/api/txn/{txn}")]
        public IActionResult CommitOrRollbackTransaction([FromRoute] TransactionRef txn, [FromBody] bool commit)
        {
            if (!commit)
            {
                _orderRm.HandleRollback(txn);
                return Ok();
            }
            var txnState = _orderRm.GetOrCreateTransactionState(txn);
            var createdOrders = new List<Order>(txnState.PendingInserts);
            _orderRm.HandleCommit(txn);
            createdOrders.ForEach(
                o => _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(o.Id)));
            return Ok();
        }
    }
}