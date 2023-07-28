namespace Bazaar.Catalog.Core.Model
{
    public class CatalogItem
    {
        // local id of catalog item
        public int Id { get; set; }
        // global product id, used across services to identify the same object
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }

        // Available stock at which we should reorder
        public int RestockThreshold { get; set; }

        // Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
        public int MaxStockThreshold { get; set; }

        public CatalogItem() { }
        public CatalogItem(CatalogItem original)
        {
            Id = original.Id;
            ProductId = original.ProductId;
            Name = original.Name;
            Description = original.Description;
            Price = original.Price;
            AvailableStock = original.AvailableStock;
            RestockThreshold = original.RestockThreshold;
            MaxStockThreshold = original.MaxStockThreshold;
        }
    }
}