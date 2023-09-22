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
        [FromQuery(Name = "productId")] string? productIds = null, string? buyerId = null)
    {
        if (string.IsNullOrWhiteSpace(productIds) && string.IsNullOrWhiteSpace(buyerId))
            return BadRequest("At least one of the following arguments must be provided: productId, buyerId.");

        List<Order> orders = new();

        if (!string.IsNullOrWhiteSpace(buyerId))
            AddOrFilterByBuyerId(ref orders, buyerId);

        if (!string.IsNullOrWhiteSpace(productIds))
            AddOrFilterByProductIds(ref orders, productIds);

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
    public ActionResult<OrderQuery> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
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

    private void AddOrFilterByProductIds(ref List<Order> orders, string joinedProductIds)
    {
        var productIds = joinedProductIds.Split(',', StringSplitOptions.TrimEntries);
        if (orders.Count == 0)
        {
            foreach (var id in productIds)
            {
                // in case there are duplicate id's in request
                if (orders.Any(o => o.Items.Any(item => item.ProductId == id)))
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
                if (filteredList.Any(o => o.Items.Any(item => item.ProductId == id)))
                    continue;

                filteredList.AddRange(orders.Where(o => o.Items.Any(item => item.ProductId == id)));
            }
            orders = filteredList;
        }
    }
}