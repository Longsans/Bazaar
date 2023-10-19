namespace Bazaar.Basket.Domain.Entites;

public class BuyerBasket
{
    public int Id { get; set; }
    public string BuyerId { get; private set; }

    private readonly List<BasketItem> _items;
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();

    public decimal Total { get; private set; }

    public BuyerBasket(string buyerId)
    {
        BuyerId = buyerId;
        _items = new();
    }

    public void AddItem(BasketItem item)
    {
        if (_items.Any(i => i.ProductId == item.ProductId))
            throw new ProductAlreadyInBasketException();

        _items.Add(item);
        UpdateTotal();
    }

    public void ChangeItemQuantity(string productId, uint quantity)
    {
        var item = _items.SingleOrDefault(i => i.ProductId == productId)
            ?? throw new ProductNotInBasketException();

        item.ChangeQuantity(quantity);

        if (item.Quantity == 0)
            _items.Remove(item);

        UpdateTotal();
    }

    public void EmptyBasket()
    {
        _items.Clear();
        UpdateTotal();
    }

    public void UpdateTotal()
    {
        Total = _items.Sum(x => x.Quantity * x.ProductUnitPrice);
    }
}