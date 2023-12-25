namespace Bazaar.Ordering.Web.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepo;
    private readonly HandleOrderService _handleOrderService;

    public OrderController(
        IOrderRepository orderRepository,
        HandleOrderService handleOrderService)
    {
        _orderRepo = orderRepository;
        _handleOrderService = handleOrderService;
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
    public ActionResult<OrderQuery> PlaceOrder(PlaceOrderRequest request)
    {
        if (!request.Items.Any())
            return BadRequest(new { error = "Order must have at least 1 item." });
        if (request.Items.Any(x => x.Quantity == 0))
            return BadRequest(new { error = "Items must have quantities of at least 1." });

        var order = new Order(
            request.ShippingAddress,
            request.BuyerId,
            request.Items.Select(x => new OrderItem(
                x.ProductId, x.ProductName, x.ProductUnitPrice, x.Quantity, default)));

        _handleOrderService.PlaceOrder(order);
        return new OrderQuery(order);
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        return _handleOrderService.UpdateOrderStatus(id, request.Status, request.CancelReason)
            .ToActionResult(this);
    }

    #region Helpers
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
    #endregion
}