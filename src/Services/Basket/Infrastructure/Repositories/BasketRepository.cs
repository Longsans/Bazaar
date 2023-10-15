namespace Bazaar.Basket.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _context;

    public BasketRepository(BasketDbContext context)
    {
        _context = context;
    }

    public BuyerBasket? GetByBuyerId(string buyerId)
    {
        return _context.BuyerBaskets
            .Include(b => b.Items)
            .SingleOrDefault(b => b.BuyerId == buyerId);
    }

    public BasketItem? GetBasketItem(string buyerId, string productId)
    {
        var basket = GetByBuyerId(buyerId);

        return basket?.Items.SingleOrDefault(x => x.ProductId == productId);
    }

    public BuyerBasket Create(string buyerId)
    {
        var basket = new BuyerBasket(buyerId);
        _context.BuyerBaskets.Add(basket);
        _context.SaveChanges();
        return basket;
    }

    public void Update(BuyerBasket basket)
    {
        _context.BuyerBaskets.Update(basket);
        _context.SaveChanges();
    }

    public BasketItem AddBasketItem(BasketItem item)
    {
        _context.BasketItems.Add(item);
        _context.SaveChanges();
        return item;
    }
}