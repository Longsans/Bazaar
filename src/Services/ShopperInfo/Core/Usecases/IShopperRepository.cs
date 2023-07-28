namespace Bazaar.ShopperInfo.Core.Usecases;

public interface IShopperRepository
{
    Shopper? GetById(int id);
    Shopper? GetByExternalId(string externalId);
    Shopper Create(Shopper shopper);
    bool Update(Shopper shopper);
    bool Delete(int id);
}
