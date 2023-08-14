namespace Bazaar.ApiGateways.WebBff.Controllers;

[ApiController]
[Route("api/[controller]s")]
public class BasketController : ControllerBase
{
    private readonly ICatalogRepository _catalogRepo;
    private readonly IBasketRepository _basketRepo;

    public BasketController(ICatalogRepository catalogRepo, IBasketRepository basketRepo)
    {
        _catalogRepo = catalogRepo;
        _basketRepo = basketRepo;
    }

    [HttpPost("{buyerId}/items/")]
    public async Task<ActionResult<Basket>> AddItemToBasket(string buyerId, BasketItemAddCommand command)
    {
        var catalogItem = await _catalogRepo.GetByProductId(command.ProductId);
        if (catalogItem == null)
        {
            return NotFound(new { error = $"Catalog item {command.ProductId} does not exist." });
        }
        if (catalogItem.AvailableStock < command.Quantity)
        {
            return BadRequest(new { error = $"Catalog item {command.ProductId} does not have enough stock to satisfy request." });
        }

        var basketItem = new BasketItem
        {
            ProductId = command.ProductId,
            ProductName = catalogItem.Name,
            UnitPrice = catalogItem.Price,
            Quantity = command.Quantity,
            ImageUrl = $"https://imgserver.com/prod-{catalogItem.Id}",
        };
        var resultBasket = await _basketRepo.AddItemToBasket(buyerId, basketItem);
        if (resultBasket == null)
        {
            return BadRequest();
        }

        return resultBasket;
    }

    [HttpPut("{buyerId}/items/{productId}")]
    public async Task<ActionResult<BasketItem>> ChangeItemQuantity(string buyerId, string productId, [FromBody] uint quantity)
    {
        if (quantity == 0)
        {
            return BadRequest("Quantity must be greater than 0.");
        }

        var catalogItem = await _catalogRepo.GetByProductId(productId);
        if (catalogItem == null)
        {
            return NotFound(new { error = $"Catalog item {productId} does not exist." });
        }
        if (catalogItem.AvailableStock < quantity)
        {
            return BadRequest(new { error = $"Catalog item {productId} does not have enough stock to satisfy request." });
        }

        var resultBasketItem = await _basketRepo.ChangeItemQuantity(buyerId, productId, quantity);
        if (resultBasketItem == null)
        {
            return NotFound(new { error = $"Product {productId} does not exist in basket of buyer {buyerId}" });
        }
        return resultBasketItem;
    }
}
