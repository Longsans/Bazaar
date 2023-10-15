namespace Bazaar.Ordering.Web.Controllers;

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
    public ActionResult<OrderQuery> CreateOrder(PlaceOrderRequest request)
    {
        if (!request.Items.Any())
            return BadRequest(new { error = "Order must have at least 1 item." });

        var order = new Order(
            request.ShippingAddress,
            request.BuyerId,
            request.Items.Select(x => new OrderItem(
                x.ProductId, x.ProductName, x.ProductUnitPrice, x.Quantity, default)));

        _orderRepo.Create(order);
        return new OrderQuery(order);
    }

    [HttpPatch("{id}")]
    public ActionResult<OrderQuery> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = _orderRepo.GetById(id);
        if (order == null)
            return NotFound(new { id });

        try
        {
            switch (request.Status)
            {
                case OrderStatus.ProcessingPayment:
                    order.StartPayment();
                    break;
                case OrderStatus.AwaitingSellerConfirmation:
                    order.AwaitSellerConfirmation();
                    break;
                case OrderStatus.Shipping:
                    order.Ship();
                    break;
                case OrderStatus.Shipped:
                    order.ConfirmShipped();
                    break;

                case OrderStatus.Postponed:
                    order.Postpone();
                    break;

                case OrderStatus.Cancelled when request.CancelReason != null:
                    order.Cancel(request.CancelReason);
                    break;
                case OrderStatus.Cancelled when request.CancelReason == null:
                    return BadRequest(new { error = "Order cancellation reason is required." });

                default:
                    return BadRequest(new { error = "Invalid status for order." });
            }
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        _orderRepo.Update(order);
        return new OrderQuery(order);
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