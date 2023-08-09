namespace Bazaar.ApiGateways.WebBff.Core.Usecases;

public interface IBasketRepository
{
    Task<Basket?> GetByBuyerId(string buyerId);
}
