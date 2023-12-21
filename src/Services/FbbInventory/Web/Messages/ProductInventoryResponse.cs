namespace Bazaar.FbbInventory.Web.Messages;

public record ProductInventoryResponse
{
    public int Id { get; }
    public string ProductId { get; }
    public bool IsStranded { get; }
    public IEnumerable<LotResponse> FulfillableLots { get; }
    public IEnumerable<LotResponse> UnfulfillableLots { get; }
    public IEnumerable<LotResponse> StrandedLots { get; }

    public uint FulfillableUnits { get; }
    public uint UnfulfillableUnits { get; }
    public uint StrandedUnits { get; }
    public uint AllUnitsInRemoval { get; }
    public uint TotalUnits { get; }
    public uint RemainingCapacity { get; }
    public uint RestockThreshold { get; }
    public uint MaxStockThreshold { get; }
    public int SellerInventoryId { get; }

    public bool HasPickupsInProgress { get; }

    public ProductInventoryResponse(ProductInventory inventory)
    {
        Id = inventory.Id;
        ProductId = inventory.ProductId;
        IsStranded = inventory.IsStranded;

        FulfillableLots = inventory.FulfillableLots.Select(x => new LotResponse(x));
        UnfulfillableLots = inventory.UnfulfillableLots.Select(x => new LotResponse(x));
        StrandedLots = inventory.StrandedLots.Select(x => new LotResponse(x));

        FulfillableUnits = (uint)FulfillableLots.Sum(x => x.UnitsInStock);
        UnfulfillableUnits = (uint)UnfulfillableLots.Sum(x => x.UnitsInStock);
        StrandedUnits = (uint)StrandedLots.Sum(x => x.UnitsInStock);
        AllUnitsInRemoval = inventory.AllUnitsInRemoval;
        TotalUnits = FulfillableUnits + UnfulfillableUnits + StrandedUnits + AllUnitsInRemoval;
        RemainingCapacity = inventory.MaxStockThreshold - TotalUnits;
        RestockThreshold = inventory.RestockThreshold;
        MaxStockThreshold = inventory.MaxStockThreshold;
        SellerInventoryId = inventory.SellerInventoryId;
        HasPickupsInProgress = inventory.HasPickupsInProgress;
    }
}
