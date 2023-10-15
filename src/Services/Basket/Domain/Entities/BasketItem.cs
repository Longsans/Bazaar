using Newtonsoft.Json;

namespace Bazaar.Basket.Domain.Entites;

public class BasketItem
{
    public int Id { get; set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal UnitPrice { get; private set; }
    public uint Quantity { get; private set; }
    public string ImageUrl { get; private set; }
    public BuyerBasket Basket { get; private set; }
    public int BasketId { get; private set; }

    [JsonConstructor]
    public BasketItem(
        string productId, string productName,
        decimal unitPrice, uint quantity,
        string imageUrl, int basketId)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        ImageUrl = imageUrl;
        BasketId = basketId;
    }

    public BasketItem(
        string productId, string productName,
        decimal unitPrice, uint quantity,
        string imageUrl, BuyerBasket basket)
            : this(
                  productId, productName, unitPrice,
                  quantity, imageUrl, basket.Id)
    {
        Basket = basket;
    }

    public void ChangeQuantity(uint quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must not be negative.");

        Quantity = quantity;
    }
}