namespace Bazaar.Transport.Web.Messages;

public class ProductInventoryResponse
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public uint NumberOfUnits { get; private set; }

    public ProductInventoryResponse(PickupProductStock productInventory)
    {
        Id = productInventory.Id;
        ProductId = productInventory.ProductId;
        NumberOfUnits = productInventory.NumberOfUnits;
    }
}
