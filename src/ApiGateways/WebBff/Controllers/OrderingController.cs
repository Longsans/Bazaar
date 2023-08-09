namespace Bazaar.ApiGateways.WebBff.Controllers
{
    [ApiController]
    public class OrderingController : ControllerBase
    {
        private readonly ICatalogRepository _catalogRepo;
        private readonly IBasketRepository _basketRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ILogger<OrderingController> _logger;

        public OrderingController(
            ICatalogRepository catalogRepo,
            IBasketRepository basketRepo,
            IOrderRepository orderRepo,
            ILogger<OrderingController> logger)
        {
            _catalogRepo = catalogRepo;
            _basketRepo = basketRepo;
            _orderRepo = orderRepo;
            _logger = logger;
        }

        [HttpPost("api/buyers/{buyerId}/orders")]
        public async Task<ActionResult<Order>> CreateOrderAsync(string buyerId)
        {
            var basket = await _basketRepo.GetByBuyerId(buyerId);
            if (basket == null)
            {
                return NotFound();
            }

            if (!basket.Items.Any())
            {
                return Conflict(new
                {
                    error = "Buyer's basket has no items.",
                    @object = basket
                });
            }

            var itemsOrdered = await _catalogRepo.GetManyByProductId(basket.Items.Select(item => item.ProductId));
            var joinedItems = Enumerable.Join(
                basket.Items,
                itemsOrdered,
                basketItem => basketItem.ProductId,
                catalogItem => catalogItem.ProductId,
                (bi, ci) => new { bi.ProductId, ci.Name, ci.AvailableStock, bi.Quantity, ci.Price });

            foreach (var item in joinedItems)
            {
                if (item.AvailableStock < item.Quantity)
                {
                    return Conflict(new { productId = item.ProductId, error = "Product's available stock does not satisfy ordered quantity." });
                }
            }

            var order = new Order
            {
                BuyerId = buyerId,
                Items = joinedItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Name,
                    ProductUnitPrice = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };
            var createdOrder = await _orderRepo.Create(order);
            return createdOrder;
        }
    }
}
