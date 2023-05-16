namespace Catalog.Model;

public interface ICatalogRepository
{
    CatalogItem? GetItemById(int id);
}