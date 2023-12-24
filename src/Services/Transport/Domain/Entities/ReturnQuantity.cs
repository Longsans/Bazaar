namespace Bazaar.Transport.Domain.Entities;

public class ReturnQuantity
{
    public int Id { get; private set; }
    public string LotNumber { get; private set; }
    public uint Quantity { get; private set; }
    public InventoryReturn Return { get; private set; }
    public int ReturnId { get; private set; }

    public ReturnQuantity(string lotNumber, uint quantity)
    {
        if (string.IsNullOrWhiteSpace(lotNumber))
        {
            throw new ArgumentNullException(nameof(lotNumber),
                "Lot number cannot be empty.");
        }
        if (quantity == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity),
                "Quantity cannot be 0.");
        }

        LotNumber = lotNumber;
        Quantity = quantity;
    }
}
