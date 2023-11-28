namespace Bazaar.Catalog.Domain.Interfaces;

public interface IDeleteCatalogItemService
{
    void AssertCanBeDeleted(CatalogItem item);
    void SoftDeleteById(int id);
    void SoftDeleteByProductId(string productId);
}
