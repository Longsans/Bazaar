namespace Bazaar.Catalog.Domain.Interfaces;

public interface IFulfillmentMethodService
{
    Result ChangeFulfillmentMethodToFbb(string productId);
    Result ChangeFulfillmentMethodToMerchant(string productId);
}
