namespace Bazaar.Catalog.Application.Specifications;

public class CatalogItemsByCriteria : Specification<CatalogItem>
{
    public CatalogItemsByCriteria(
        string? category = null, string? sellerId = null,
        string? nameSubstring = null, string? productId = null,
        bool includeDeleted = false)
    {
        if (string.IsNullOrWhiteSpace(productId)
            && string.IsNullOrWhiteSpace(sellerId)
            && string.IsNullOrWhiteSpace(nameSubstring)
            && string.IsNullOrWhiteSpace(category))
        {
            throw new ArgumentException("At least one of the following query parameters must be specified: " +
                "productId, sellerId, partOfName, category");
        }

        Query.Include(x => x.MainDepartment)
            .Include(x => x.Subcategory);
        if (category is not null)
            Query.Where(x => x.MainDepartment.Name.Contains(category) || x.Subcategory.Name.Contains(category));

        if (sellerId is not null)
            Query.Where(x => x.SellerId == sellerId);

        if (nameSubstring is not null)
            Query.Where(x => x.ProductName.Contains(nameSubstring));

        if (productId is not null)
            Query.Where(x => x.ProductId == productId);

        if (!includeDeleted)
            Query.Where(x => x.ListingStatus != ListingStatus.Deleted);
    }
}
