namespace WebShoppingUI.DataServices;

public interface IShopperInfoDataService
{
    Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId);
    Task<ServiceCallResult> UpdateInfo(string externalId, ShopperWriteCommand updateCommand);
    Task<ServiceCallResult<Shopper>> Register(ShopperWriteCommand shopperInfo);
}
