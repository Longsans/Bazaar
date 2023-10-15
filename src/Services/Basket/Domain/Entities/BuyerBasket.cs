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

    public void RemoveNoQuantityItems()
    {
        _items.RemoveAll(i => i.Quantity == 0);
    }

    public void EmptyBasket()
    {
        _items.Clear();
    }

    public void UpdateTotal(decimal total)
    {
        Total = total >= 0m ? total : throw new ArgumentException(
            "Basket total cannot be negative", nameof(total));
    }
}