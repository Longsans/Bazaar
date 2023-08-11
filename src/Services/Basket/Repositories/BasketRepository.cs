namespace Bazaar.Basket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly BasketDbContext _context;

    public BasketRepository(BasketDbContext context)
    {
        _context = context;
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

    public BasketItem? GetBasketItem(string buyerId, string productId)
    {
        var basket = GetByBuyerId(buyerId);
        if (basket == null)
        {
            return null;
        }

        return basket.Items.FirstOrDefault(x => x.ProductId == productId);
    }

    private BuyerBasket? GetByBuyerId(string buyerId)
    {
        return _context.BuyerBaskets
            .Include(b => b.Items)
            .FirstOrDefault(b => b.BuyerId == buyerId);
    }

    public IAddItemToBasketResult AddItemToBasket(string buyerId, BasketItem item)
    {
        var basket = GetBasketOrCreateIfNotExist(buyerId);
        var existingItem = basket.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
        if (existingItem != null)
        {
            return IAddItemToBasketResult.BasketItemAlreadyAddedError;
        }

        basket.Items.Add(item);
        _context.SaveChanges();
        return IAddItemToBasketResult.Success(basket);
    }

    public IChangeItemQuantityResult ChangeItemQuantity(string buyerId, string productId, uint quantity)
    {
        if (quantity < 1)
        {
            return IChangeItemQuantityResult.QuantityLessThanOneError;
        }

        var basket = GetBasketOrCreateIfNotExist(buyerId);
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

    public IRemoveItemFromBasketResult RemoveItemFromBasket(string buyerId, string productId)
    {
        var basket = GetBasketOrCreateIfNotExist(buyerId);
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