namespace WebShoppingUI.HttpServices;

public interface IShopperInfoHttpService
{
    Task<ServiceCallResult<Shopper>> GetByExternalId(string externalId);
}
