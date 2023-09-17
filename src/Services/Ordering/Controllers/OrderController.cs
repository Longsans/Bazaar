namespace Bazaar.Ordering.Adapters.Controllers;

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
    public ActionResult<IEnumerable<OrderQuery>> GetByCriteria(
        string? productId = null, string? buyerId = null)
    {
        if (string.IsNullOrWhiteSpace(productId) && string.IsNullOrWhiteSpace(buyerId))
            return BadRequest("At least one of the following arguments must be provided: productId, buyerId.");

        List<Order> orders = new();

        if (!string.IsNullOrWhiteSpace(buyerId))
            AddOrFilterByBuyerId(ref orders, buyerId);

        if (!string.IsNullOrWhiteSpace(productId))
            AddOrFilterByProductId(ref orders, productId);

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

    private void AddOrFilterByBuyerId(ref List<Order> orders, string buyerId)
    {
        if (orders.Count == 0)
        {
            orders.AddRange(_orderRepo.GetByBuyerId(buyerId));
            return;
        }
        orders = orders.Where(o => o.BuyerId == buyerId).ToList();
    }

    private void AddOrFilterByProductId(ref List<Order> orders, string productId)
    {
        var productIds = productId.Split(',', StringSplitOptions.TrimEntries);
        var productInOrder = (Order o) => o.Items.Any(item => item.ProductId == productId);
        if (orders.Count == 0)
        {
            foreach (var id in productIds)
            {
                // in case there are duplicate id's in request
                if (orders.Any(productInOrder))
                {
                    continue;
                }
                orders.AddRange(_orderRepo.GetByProductId(id));
            }
        }
        else
        {
            var filteredList = new List<Order>(orders.Count);
            foreach (var id in productIds)
            {
                if (filteredList.Any(productInOrder))
                    continue;

                filteredList.AddRange(orders.Where(productInOrder));
            }
            orders = filteredList;
        }
    }
}