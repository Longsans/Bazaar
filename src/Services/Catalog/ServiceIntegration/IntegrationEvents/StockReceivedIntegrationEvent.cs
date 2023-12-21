namespace Bazaar.Catalog.ServiceIntegration.IntegrationEvents;

public record StockReceivedIntegrationEvent : IntegrationEvent
{
    public DateTime DateOfReceipt { get; }
    public IEnumerable<ReceivedStockEventItem> Items { get; }

    public StockReceivedIntegrationEvent(DateTime dateOfReceipt, IEnumerable<ReceivedStockEventItem> items)
    {
        DateOfReceipt = dateOfReceipt;
        Items = items;
    }
}

public record ReceivedStockEventItem
{
    public string ProductId { get; }
    public uint GoodQuantity { get; }
    public uint DefectiveQuantity { get; }
    public uint WarehouseDamagedQuantity { get; }

    public ReceivedStockEventItem(
        string productId, uint goodQuantity,
        uint defectiveQuantity, uint warehouseDamagedQuantity)
    {
        ProductId = productId;
        GoodQuantity = goodQuantity;
        DefectiveQuantity = defectiveQuantity;
        WarehouseDamagedQuantity = warehouseDamagedQuantity;
    }
}