namespace Bazaar.Transport.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint NumberOfUnits { get; private set; }
    public InventoryPickup Pickup { get; private set; }
    public int PickupId { get; private set; }

    public ProductInventory(string productId, uint numberOfUnits)
    {
        if (numberOfUnits == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfUnits), "Number of stock units cannot be 0.");
        }

        ProductId = productId;
        NumberOfUnits = numberOfUnits;
    }
}
