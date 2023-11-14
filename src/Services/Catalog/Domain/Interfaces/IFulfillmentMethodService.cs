namespace Bazaar.Catalog.Domain.Interfaces;

public interface IFulfillmentMethodService
{
    Result ChangeToFulfillmentByBazaar(string productId);
    Result ChangeToFulfillmentByMerchant(string productId);
}
