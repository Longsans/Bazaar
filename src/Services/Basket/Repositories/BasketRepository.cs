namespace Bazaar.Basket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _context;

    public BasketRepository(BasketDbContext context)
    {
        _context = context;
    }

    public BuyerBasket? AddItemToBasket(string buyerId, BasketItem item)
    {
        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return null;
        }
        basket.Items.Add(item);
        _context.SaveChanges();
        return basket;
    }

    public BuyerBasket? ChangeItemQuantity(string buyerId, string productId, uint quantity)
    {
        if (quantity == 0)
        {
            return null;
        }

        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return null;
        }

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            return null;
        }

        item.Quantity = quantity;
        _context.SaveChanges();
        return basket;
    }

    public BuyerBasket GetBasketOrCreateIfNotExist(string buyerId)
    {
        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            basket = new BuyerBasket(buyerId)
            {
                Items = new()
            };
            _context.BuyerBaskets.Add(basket);
            _context.SaveChanges();
        }
        return basket;
    }

    public BuyerBasket? GetByBuyerId(string buyerId)
    {
        return _context.BuyerBaskets
            .Include(b => b.Items)
            .FirstOrDefault(b => b.BuyerId == buyerId);
    }

    public BuyerBasket? RemoveItemFromBasket(string buyerId, string productId)
    {
        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return null;
        }

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            return null;
        }

        basket.Items.Remove(item);
        _context.SaveChanges();
        return basket;
    }
}