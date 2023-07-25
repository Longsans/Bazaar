namespace Bazaar.Basket.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly CustomerBasket[] _baskets;

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
        _baskets = new CustomerBasket[] {
            new CustomerBasket("CUST-1", items)
        };
    }

    public CustomerBasket? GetByBuyerId(string buyerId)
    {
        return _baskets.FirstOrDefault(b => b.BuyerId == buyerId);
    }
}