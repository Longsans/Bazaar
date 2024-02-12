namespace Bazaar.FbbInventory.Domain.Entities;

public class SellerInventory
{
    public int Id { get; private set; }
    public string SellerId { get; private set; }
    public List<ProductInventory> ProductInventories { get; private set; }
    public float TotalStorageSpaceUsedM3
    {
        get => ProductInventories.Sum(x => x.TotalStorageSpaceM3);
        private set { } // same as product inventory and lot
    }

    public SellerInventory(string sellerId)
    {
        SellerId = sellerId;
        ProductInventories = new();
    }
}
