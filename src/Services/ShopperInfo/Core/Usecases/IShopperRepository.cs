namespace Bazaar.ShopperInfo.Core.Usecases;

public interface IShopperRepository
{
    Shopper? GetById(int id);
    Shopper? GetByExternalId(string externalId);
    Shopper Register(Shopper shopper);
    bool UpdateInfoByExternalId(Shopper shopper);
    bool Delete(int id);
    bool DeleteByExternalId(string externalId);
}
