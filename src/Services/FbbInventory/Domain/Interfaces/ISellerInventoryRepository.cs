namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface ISellerInventoryRepository
{
    SellerInventory? GetWithProductsById(int id);
    SellerInventory? GetWithProductsBySellerId(string sellerId);
    void Update(SellerInventory sellerInventory);
}
