namespace Bazaar.FbbInventory.Domain.Interfaces;

public interface ISellerInventoryRepository
{
    IEnumerable<SellerInventory> GetAll();
    SellerInventory? GetWithProductsById(int id);
    SellerInventory? GetWithProductsBySellerId(string sellerId);
    void Update(SellerInventory sellerInventory);
}
