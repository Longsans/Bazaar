namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class InboundStockQuantity
{
    public string ProductId { get; }
    public uint Quantity { get; }

    public InboundStockQuantity(string productId, uint quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.");
        }
        if (quantity == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity cannot be 0.");
        }

        ProductId = productId;
        Quantity = quantity;
    }
}
