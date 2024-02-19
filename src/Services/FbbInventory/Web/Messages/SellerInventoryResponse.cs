namespace Bazaar.FbbInventory.Web.Messages;

public record SellerInventoryResponse(int Id, string SellerId, float TotalStorageSpaceUsedInM3, IEnumerable<ProductInventoryResponse> ProductInventories)
{
    public SellerInventoryResponse(SellerInventory inventory) : this(inventory.Id, inventory.SellerId,
        inventory.TotalStorageSpaceUsedM3, inventory.ProductInventories.Select(x => new ProductInventoryResponse(x)))
    {
    }
}
