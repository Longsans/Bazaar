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

    public IChangeItemQuantityResult ChangeItemQuantity(string buyerId, string productId, uint quantity)
    {
        if (quantity < 1)
        {
            return IChangeItemQuantityResult.QuantityLessThanOneError;
        }

        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return IChangeItemQuantityResult.BasketNotFoundError;
        }

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            return IChangeItemQuantityResult.BasketItemNotFoundError;
        }

        item.Quantity = quantity;
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return IChangeItemQuantityResult.OtherExceptionError(ex.Message);
        }
        return IChangeItemQuantityResult.Success(item);
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

    public IRemoveItemFromBasketResult RemoveItemFromBasket(string buyerId, string productId)
    {
        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return IRemoveItemFromBasketResult.BasketNotFoundError;
        }

        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            return IRemoveItemFromBasketResult.BasketItemNotFoundError;
        }

        basket.Items.Remove(item);
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            return IRemoveItemFromBasketResult.OtherExceptionError(ex.Message);
        }
        return IRemoveItemFromBasketResult.Success(basket);
    }
}