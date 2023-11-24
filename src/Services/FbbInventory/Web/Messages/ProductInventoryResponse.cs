﻿namespace Bazaar.FbbInventory.Web.Messages;

public record ProductInventoryResponse
{
    public int Id { get; init; }
    public string ProductId { get; init; }
    public List<FulfillableLotResponse> FulfillableLots { get; init; }
    public List<UnfulfillableLotResponse> UnfulfillableLots { get; init; }

    public uint FulfillableUnitsInStock { get; init; }
    public uint FulfillableUnitsPendingRemoval { get; init; }
    public uint UnfulfillableUnitsInStock { get; init; }
    public uint UnfulfillableUnitsPendingRemoval { get; init; }
    public uint TotalUnits { get; init; }
    public uint RestockThreshold { get; init; }
    public uint MaxStockThreshold { get; init; }
    public int SellerInventoryId { get; init; }

    public bool HasPickupsInProgress { get; init; }

    public ProductInventoryResponse(ProductInventory inventory)
    {
        Id = inventory.Id;
        ProductId = inventory.ProductId;
        FulfillableLots = inventory.FulfillableLots
            .Select(x => new FulfillableLotResponse(x)).ToList();
        UnfulfillableLots = inventory.UnfulfillableLots
            .Select(x => new UnfulfillableLotResponse(x)).ToList();

        FulfillableUnitsInStock = inventory.FulfillableUnitsInStock;
        FulfillableUnitsPendingRemoval = inventory.FulfillableUnitsPendingRemoval;
        UnfulfillableUnitsInStock = inventory.UnfulfillableUnitsInStock;
        UnfulfillableUnitsPendingRemoval = inventory.UnfulfillableUnitsPendingRemoval;
        TotalUnits = inventory.TotalUnits;
        RestockThreshold = inventory.RestockThreshold;
        MaxStockThreshold = inventory.MaxStockThreshold;
        SellerInventoryId = inventory.SellerInventoryId;
        HasPickupsInProgress = inventory.HasPickupsInProgress;
    }
}