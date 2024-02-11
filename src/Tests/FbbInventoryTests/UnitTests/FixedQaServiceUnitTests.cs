using Bazaar.FbbInventory.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace FbbInventoryTests.UnitTests;

public class FixedQaServiceUnitTests
{
    private readonly FixedStockInspectionService _service;
    private const uint _defectiveUnitsPerItem = 5;
    private const uint _wdamagedUnitsPerItem = 6;

    public FixedQaServiceUnitTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                {"QaDefectivePerItem", _defectiveUnitsPerItem.ToString() },
                {"QaWarehouseDamagedPerItem", _wdamagedUnitsPerItem.ToString() }
            })
            .Build();
        _service = new FixedQualityInspectionService(config);
    }

    [Fact]
    public void ConductInspection_ReturnsReportWithFixedDefectiveAndWarehouseDamagedItems_WhenEnoughUnits()
    {
        var quantities = new List<InboundStockQuantity>
        {
            new("PROD-1", 15),
            new("PROD-2", 25),
            new("PROD-3", 35),
        };

        var report = _service.ConductInspection(quantities);

        foreach (var inspectionItem in report.Items)
        {
            var inboundQty = quantities.Single(x => x.ProductId == inspectionItem.ProductId);
            Assert.Equal(
                inboundQty.Quantity - _defectiveUnitsPerItem - _wdamagedUnitsPerItem,
                inspectionItem.GoodQuantity);
            Assert.Equal(_defectiveUnitsPerItem, inspectionItem.DefectiveQuantity);
            Assert.Equal(_wdamagedUnitsPerItem, inspectionItem.WarehouseDamagedQuantity);
        }
    }

    [Fact]
    public void ConductInspection_ReturnsReportWithFixedDefectiveAndWarehouseDamagedItems_WhenNotEnoughUnits()
    {
        var quantities = new List<InboundStockQuantity>
        {
            new("PROD-1", 4),
            new("PROD-2", 8),
            new("PROD-3", 12),
        };

        var report = _service.ConductInspection(quantities);

        var prod1Item = report.Items.Single(x => x.ProductId == "PROD-1");
        var prod2Item = report.Items.Single(x => x.ProductId == "PROD-2");
        var prod3Item = report.Items.Single(x => x.ProductId == "PROD-3");
        AssertReportQuantities(prod1Item, 4, 0, 0);
        AssertReportQuantities(prod2Item, 3, 5, 0);
        AssertReportQuantities(prod3Item, 1, 5, 6);
    }

    private static void AssertReportQuantities(StockInspectionItem inspectionItem,
        uint goodQty, uint defectiveQty, uint wdmgQty)
    {
        Assert.Equal(goodQty, inspectionItem.GoodQuantity);
        Assert.Equal(defectiveQty, inspectionItem.DefectiveQuantity);
        Assert.Equal(wdmgQty, inspectionItem.WarehouseDamagedQuantity);
    }
}
