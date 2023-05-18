using System.Linq;
using Bazaar.Catalog.Model;
namespace Bazaar.Catalog.Repositories;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogItem[] _items = {
        new CatalogItem
        {
            Id = 1,
            ProductId = "PROD-1",
            Name = "The Winds of Winter",
            Description = "Book 6 of ASOIAF",
            Price = 34.99m,
            AvailableStock = 32,
            RestockThreshold = 30,
            MaxStockThreshold = 1000,
        },
        new CatalogItem
        {
            Id = 2,
            ProductId = "PROD-2",
            Name = "A Dream of Spring",
            Description = "Last book in ASOIAF",
            Price = 45.99m,
            AvailableStock = 45,
            RestockThreshold = 30,
            MaxStockThreshold = 1000,
        }
    };

    public CatalogItem? GetItemById(int id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }
}