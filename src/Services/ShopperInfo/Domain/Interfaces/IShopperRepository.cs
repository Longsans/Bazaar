namespace Bazaar.ShopperInfo.Domain.Interfaces;

public interface IShopperRepository
{
    Shopper? GetById(int id);
    Shopper? GetByExternalId(string externalId);
    Shopper Create(Shopper shopper);
    void Update(Shopper shopper);
}
