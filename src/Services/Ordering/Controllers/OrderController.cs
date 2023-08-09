namespace Bazaar.Ordering.Adapters.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEventBus _eventBus;

        public OrderController(
            IOrderRepository orderRepository,
            IEventBus eventBus)
        {
            _orderRepo = orderRepository;
            _eventBus = eventBus;
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
            var createResult = _orderRepo.Create(command.ToOrder());
            if (createResult is OrderHasNoItemsError)
            {
                return BadRequest(new
                {
                    error = "Order must have at least 1 item.",
                    @object = command
                });
            }

            var order = createResult switch
            {
                OrderSuccessResult r => r.Order,
                _ => new Order()
            };
            _eventBus.Publish(
                new OrderCreatedIntegrationEvent(
                    order.Id, order.Items.Select(i => new OrderStockItem(i.ProductId, i.Quantity))));

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
    }
}