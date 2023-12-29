namespace Bazaar.FbbInventory.Domain.ValueObjects;

public class OutboundStockQuantity
{
    public string ProductId { get; }
    public uint GoodQuantity { get; }
    public uint UnfulfillableQuantity { get; }

    public OutboundStockQuantity(string productId,
        uint goodQuantity, uint unfulfillableQuantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("Product ID cannot be empty.");
        }
        if (goodQuantity + unfulfillableQuantity == 0)
        {
            throw new ArgumentException("Total quantity cannot be 0.");
        }

        ProductId = productId;
        GoodQuantity = goodQuantity;
        UnfulfillableQuantity = unfulfillableQuantity;
    }
}
