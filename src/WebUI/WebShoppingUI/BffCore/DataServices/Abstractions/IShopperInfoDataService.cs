namespace WebShoppingUI.DataServices;

public interface IShopperInfoDataService
{
    Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId);
    Task<ServiceCallResult> UpdateInfo(string externalId, ShopperPersonalInfo updateCommand);
    Task<ServiceCallResult<Shopper>> Register(ShopperRegistration shopperInfo);
    Task<ServiceCallResult> ChangeEmailAddress(string externalId, string emailAddress);
}
