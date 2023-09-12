using Microsoft.AspNetCore.Mvc;

namespace WebSellerUI.Controllers;

[Route("api/orders")]
[ApiController]
public class OrderingController : ControllerBase
{
    private readonly OrderingService _orderingSvc;
    private readonly CatalogService _catalogSvc;

    public OrderingController(OrderingService orderingService, CatalogService catalogService)
    {
        _orderingSvc = orderingService;
        _catalogSvc = catalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetBySellerId([FromQuery] string sellerId)
    {
        if (string.IsNullOrWhiteSpace(sellerId))
        {
            return BadRequest("Request must include one or more seller IDs.");
        }

        var orders = new List<Order>();
        var sellers = sellerId.Split(',', StringSplitOptions.TrimEntries);
        foreach (var seller in sellers)
        {
            var products = await _catalogSvc.GetBySellerId(seller);
            if (products == null)
            {
                return Unauthorized();
            }
            var productIdsJoined = string.Join(',', products.Select(x => x.ProductId));
            orders.AddRange(await _orderingSvc.GetByProductIds(productIdsJoined));
        }
        return orders;
    }
}
