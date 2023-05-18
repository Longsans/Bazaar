using Microsoft.AspNetCore.Mvc;

namespace Bazaar.Ordering.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IEventBus _eventBus;

        public OrderController(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepo = orderRepository;
            _eventBus = eventBus;
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
        public ActionResult<Order> PostAsync([FromBody] OrderCreateCommand createOrderCommand)
        {
            try
            {
                var createdOrder = _orderRepo.CreateProcessingPayment(new Order(createOrderCommand));
                _eventBus.Publish(new OrderStatusChangedToProcessingPaymentIntegrationEvent(createdOrder.Id));
                return Ok(createdOrder);
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