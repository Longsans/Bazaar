namespace WebShoppingUI.DataServices;

public interface IShopperInfoDataService
{
    Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId);
}
