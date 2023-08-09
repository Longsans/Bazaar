namespace Bazaar.ApiGateways.WebBff.Core.Usecases;

public interface ICatalogRepository
{
    Task<CatalogItem?> GetByProductId(string productId);
    Task<IEnumerable<CatalogItem>> GetManyByProductId(IEnumerable<string> productIds);
}
