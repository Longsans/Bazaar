namespace Bazaar.Ordering.Adapters.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(
            IOrderRepository orderRepository)
        {
            _orderRepo = orderRepository;
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
        public ActionResult<IEnumerable<OrderQuery>> GetAll([FromQuery] string? productId = null)
        {
            List<Order> orders = new();
            if (productId != null)
            {
                var ids = productId.Split(',', StringSplitOptions.TrimEntries);
                foreach (var id in ids)
                {
                    if (orders.Any(o => o.Items.Any(item => item.ProductId == id)))
                    {
                        continue;
                    }
                    orders.AddRange(_orderRepo.GetByProductId(id));
                }
            }
            else
            {
                orders = _orderRepo.GetAll().ToList();
            }
            return Ok(orders.Select(o => new OrderQuery(o)));
        }

        [HttpPost]
        public ActionResult<OrderQuery> CreateOrder(OrderAddCommand command)
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

            return createResult switch
            {
                OrderSuccessResult r => CreatedAtAction(nameof(GetById), new { id = r.Order.Id }, new OrderQuery(r.Order)),
                _ => StatusCode(500)
            };
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