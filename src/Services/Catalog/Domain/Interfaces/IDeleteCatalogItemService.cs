namespace Bazaar.Catalog.Domain.Interfaces;

public interface IDeleteCatalogItemService
{
    Result SoftDeleteById(int id);
    Result SoftDeleteByProductId(string productId);
}
