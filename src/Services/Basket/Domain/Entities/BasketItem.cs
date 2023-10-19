using Newtonsoft.Json;

namespace Bazaar.Basket.Domain.Entites;

public class BasketItem
{
    public int Id { get; set; }
    public string ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal ProductUnitPrice { get; private set; }
    public uint Quantity { get; private set; }
    public string ImageUrl { get; private set; }
    public BuyerBasket Basket { get; private set; }
    public int BasketId { get; private set; }

    [JsonConstructor]
    public BasketItem(
        string productId, string productName,
        decimal productUnitPrice, uint quantity,
        string imageUrl, int basketId)
    {
        if (productUnitPrice <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(productUnitPrice), "Unit price must be higher than 0.");
        }

        if (quantity == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be larger than 0.");
        }

        ProductId = productId;
        ProductName = productName;
        ProductUnitPrice = productUnitPrice;
        Quantity = quantity;
        ImageUrl = imageUrl;
        BasketId = basketId;
    }

    public BasketItem(
        string productId, string productName,
        decimal productUnitPrice, uint quantity,
        string imageUrl, BuyerBasket basket)
            : this(
                  productId, productName, productUnitPrice,
                  quantity, imageUrl, basket.Id)
    {
        Basket = basket;
    }

    // Allow zero quantity here, which will be interpreted as removing item by the basket
    public void ChangeQuantity(uint quantity)
    {
        Quantity = quantity;
    }
}