using Bazaar.BuildingBlocks.Transactions.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult<OrderQuery> Get(int id)
        {
            var order = _orderRepo.GetById(id);
            if (order == null)
                return NotFound();
            return new OrderQuery(order);
        }

        [HttpGet("latest")]
        public ActionResult<OrderQuery> GetLatest()
        {
            var order = _orderRepo.GetLatest();
            if (order == null)
                return NotFound();
            return new OrderQuery(order);
        }

        [HttpPut]
        public IActionResult Put([FromBody] Order order)
        {
            _orderRepo.Update(order);
            return Ok();
        }

        [HttpPost("txn/{txn}")]
        public ActionResult<OrderQuery> Post([FromRoute] TransactionRef txn, [FromBody] OrderCreateCommand createOrderCommand)
        {
            try
            {
                var txnState = _orderRm.GetOrCreateTransactionState(txn);
                var createdOrder = new Order(createOrderCommand)
                {
                    Id = _orderRepo.NextOrderId,
                    Status = OrderStatus.ProcessingPayment
                };
                createdOrder.AssignExternalId();
                txnState.PendingInserts.Add(createdOrder);
                return new OrderQuery(createdOrder);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("txn/prepare")]
        public IActionResult PrepareToCommitTransaction([FromBody] TransactionRef txn)
        {
            _orderRm.HandlePrepare(txn);
            return Ok();
        }

        [HttpPost("txn/commit")]
        public IActionResult CommitTransaction([FromBody] TransactionRef txn)
        {
            var txnState = _orderRm.GetOrCreateTransactionState(txn);
            var createdOrders = new List<Order>(txnState.PendingInserts);
            _orderRm.HandleCommit(txn);
            createdOrders.ForEach(
                o => _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(o.Id)));
            return Ok();
        }
    }
}