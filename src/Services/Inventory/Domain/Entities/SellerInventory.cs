namespace Bazaar.Inventory.Domain.Entities;

public class SellerInventory
{
    public int Id { get; private set; }
    public string SellerId { get; private set; }
    public List<ProductInventory> ProductInventories { get; private set; }

    public SellerInventory(string sellerId)
    {
        SellerId = sellerId;
        ProductInventories = new();
    }
}
