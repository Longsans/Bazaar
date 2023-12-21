namespace Bazaar.FbbInventory.Domain.Entities;

public class SellerInventory
{
    public int Id { get; private set; }
    public string SellerId { get; private set; }
    public List<ProductInventory> ProductInventories { get; private set; }

    public SellerInventory(string sellerId)
    {
        SellerId = sellerId;
        ProductInventories = new();
    }

    public void ReceiveStock(StockReceipt receipt)
    {
        foreach (var item in receipt.Items)
        {
            var totalQty = item.GoodQuantity + item.DefectiveQuantity
                + item.WarehouseDamagedQuantity;
            if (totalQty == 0)
            {
                throw new ArgumentException($"Item of product with ID {item.ProductId} has zero quantity. " +
                    "Each item must have at least 1 unit.");
            }

            var inventory = ProductInventories.SingleOrDefault(x => x.ProductId == x.ProductId)
                ?? throw new ArgumentException($"Inventory not found for product ID: {item.ProductId}." +
                    "All stock items must be of products of this seller inventory.");
            if (inventory.RemainingCapacity < totalQty)
            {
                throw new ExceedingMaxStockThresholdException();
            }

            if (item.GoodQuantity > 0)
            {
                if (!inventory.IsStranded)
                {
                    inventory.ReceiveFulfillableStock(item.GoodQuantity);
                }
                else
                {
                    inventory.ReceiveStrandedStock(item.GoodQuantity);
                }
            }
            if (item.DefectiveQuantity > 0)
            {
                inventory.ReceiveUnfulfillableStock(
                    UnfulfillableCategory.Defective, item.DefectiveQuantity);
            }
            if (item.WarehouseDamagedQuantity > 0)
            {
                inventory.ReceiveUnfulfillableStock(
                    UnfulfillableCategory.WarehouseDamaged, item.WarehouseDamagedQuantity);
            }
        }
    }

    /// <summary>
    /// Issues stock from product inventories, starting from older units and ending with newer units.
    /// </summary>
    /// <param name="issueQuantities"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotEnoughUnitsException"></exception>
    public StockIssue IssueStock(IEnumerable<OutboundStockQuantity> issueQuantities, StockIssueReason issueReason)
    {
        if (!issueQuantities.Any())
        {
            throw new ArgumentException("Issue items list cannot be empty.");
        }
        if (issueQuantities.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
        {
            throw new ArgumentException("Duplicate products. " +
                "Each product can only appear once in a stock issue.");
        }

        StockIssue? issue = null;
        foreach (var issuedStock in issueQuantities)
        {
            if (issuedStock.GoodQuantity + issuedStock.UnfulfillableQuantity == 0)
            {
                throw new ArgumentException(
                    "Each issue item must have at least 1 unit.");
            }

            var inventory = ProductInventories.SingleOrDefault(x => x.ProductId == issuedStock.ProductId)
                ?? throw new ArgumentException($"Inventory not found for product ID: {issuedStock.ProductId}." +
                    "All issue items must be of products of this seller inventory.");
            var invenIssue = inventory.IssueStockByDateOldToNew(
                issuedStock.GoodQuantity, issuedStock.UnfulfillableQuantity, issueReason);
            issue = issue == null ? invenIssue : issue.AddItems(invenIssue.Items);
        }
        return issue!;
    }
}
