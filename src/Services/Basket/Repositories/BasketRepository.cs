namespace Bazaar.Basket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly List<CustomerBasket> _baskets;

    public BasketRepository()
    {
        var items = new BasketItem[] {
                new BasketItem {
                    Id = 1,
                    ProductId = "PROD-1",
                    ProductName = "The Winds of Winter",
                    UnitPrice = 34.99m,
                    Quantity = 2,
                    ImageUrl = "https://imageserver.com/twow-when?"
                },
                new BasketItem {
                    Id = 1,
                    ProductId = "PROD-2",
                    ProductName = "A Dream of Spring",
                    UnitPrice = 45.99m,
                    Quantity = 1,
                    ImageUrl = "https://imageserver.com/not-coming"
                }
            };
        _baskets = new List<CustomerBasket> {
            new CustomerBasket("CUST-1", items)
        };
    }

    public CustomerBasket GetBasketOrCreateIfNotExist(string buyerId)
    {
        var basket = _baskets.FirstOrDefault(b => b.BuyerId == buyerId);
        if (basket == null)
        {
            basket = new CustomerBasket(buyerId);
            _baskets.Add(basket);
        }
        return basket;
    }

    public CustomerBasket? GetByBuyerId(string buyerId)
    {
        return _baskets.FirstOrDefault(b => b.BuyerId == buyerId);
    }

    public void Update(string buyerId, CustomerBasket update)
    {
        var basket = _baskets.FirstOrDefault(b => b.BuyerId == buyerId);
        if (basket != null)
        {
            _baskets.Remove(basket);
        }
        _baskets.Add(update);
    }
}