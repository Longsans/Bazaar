namespace Bazaar.FbbInventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }
    public float StorageLengthPerUnitCm { get; private set; } // Length is the measured longest edge of the item 
    public float StorageWidthPerUnitCm { get; private set; }  // Width is the second longest edge
    public float StorageHeightPerUnitCm { get; private set; } // Height is the shortest edge
    public float StorageSpacePerUnitCm3 => StorageWidthPerUnitCm * StorageLengthPerUnitCm * StorageHeightPerUnitCm;
    public float TotalStorageSpaceCm3
    {
        get => TotalUnits * StorageSpacePerUnitCm3;
        private set { } // private set for EF mapping to store in DB
    }
    public float TotalStorageSpaceM3 => ConvertUnits.FromCm3ToM3(TotalStorageSpaceCm3);

    private readonly List<Lot> _lots = new();
    public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();
    public IReadOnlyCollection<Lot> FulfillableLots
        => _lots.Where(x => x.IsUnitsFulfillable).ToList().AsReadOnly();
    public IReadOnlyCollection<Lot> UnfulfillableLots
        => _lots.Where(x => x.IsUnitsUnfulfillable).ToList().AsReadOnly();
    public IReadOnlyCollection<Lot> StrandedLots
        => _lots.Where(x => x.IsUnitsStranded).ToList().AsReadOnly();

    public uint FulfillableUnits => (uint)FulfillableLots.Sum(x => x.UnitsInStock);
    public uint UnfulfillableUnits => (uint)UnfulfillableLots.Sum(x => x.UnitsInStock);
    public uint StrandedUnits => (uint)StrandedLots.Sum(x => x.UnitsInStock);
    public uint AllUnitsInRemoval => (uint)_lots.Sum(x => x.UnitsInRemoval);
    public uint TotalUnits => FulfillableUnits + UnfulfillableUnits + StrandedUnits + AllUnitsInRemoval;
    public uint RemainingCapacity => MaxStockThreshold - TotalUnits;

    public uint RestockThreshold { get; private set; }
    public uint MaxStockThreshold { get; private set; }
    public SellerInventory SellerInventory { get; private set; }
    public int SellerInventoryId { get; private set; }

    public bool IsStranded { get; private set; }
    public bool HasPickupsInProgress { get; private set; }

    public ProductInventory(
        string productId, uint fulfillableUnits, uint defectiveUnits, uint warehouseDamagedUnits, uint restockThreshold, uint maxStockThreshold,
        float storageLengthPerUnitCm, float storageWidthPerUnitCm, float storageHeightPerUnitCm, int sellerInventoryId)
    {
        if (restockThreshold > maxStockThreshold
            || fulfillableUnits + defectiveUnits + warehouseDamagedUnits > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;
        StorageWidthPerUnitCm = storageWidthPerUnitCm;
        StorageLengthPerUnitCm = storageLengthPerUnitCm;
        StorageHeightPerUnitCm = storageHeightPerUnitCm;
        if (StorageSpacePerUnitCm3 <= 0f)
        {
            throw new ArgumentOutOfRangeException("Storage space per unit must be a positive number.", null as Exception);
        }

        if (fulfillableUnits != 0)
        {
            _lots.Add(new(this, fulfillableUnits));
        }
        if (defectiveUnits != 0)
        {
            _lots.Add(new(this, defectiveUnits, UnfulfillableCategory.Defective));
        }
        if (warehouseDamagedUnits != 0)
        {
            _lots.Add(new(this, warehouseDamagedUnits, UnfulfillableCategory.WarehouseDamaged));
        }
    }

    // EF read constructor
    private ProductInventory() { }

    #region Domain logic
    public IEnumerable<Lot> GetGoodLotsFifoForStockDemand(uint requiredUnits)
    {
        return GetLotsFifoForStockDemand(requiredUnits, lot => !lot.IsUnitsUnfulfillable);
    }

    public IEnumerable<Lot> GetUnfulfillabbleLotsFifoForStockDemand(uint requiredUnits)
    {
        return GetLotsFifoForStockDemand(requiredUnits, lot => lot.IsUnitsUnfulfillable);
    }

    public void ReceiveGoodStock(uint units)
    {
        if (!IsStranded)
        {
            AddLotOrIncreaseLotUnits(units,
                () => _lots.SingleOrDefault(x => x.IsUnitsFulfillable
                    && x.DateUnitsEnteredStorage == DateTime.Now.Date),
                () => new Lot(this, units));
        }
        else
        {
            AddLotOrIncreaseLotUnits(units,
                () => _lots.SingleOrDefault(x => x.DateUnitsEnteredStorage == DateTime.Now.Date
                    && x.DateUnitsBecameStranded == DateTime.Now.Date),
                () => new Lot(this, units, DateTime.Now.Date, DateTime.Now.Date));
        }
    }

    public void AddUnfulfillableStock(uint units, DateTime dateUnitsEnteredStorage,
        DateTime dateUnitsBecameUnfulfillable, UnfulfillableCategory unfulfillableCategory)
    {
        AddLotOrIncreaseLotUnits(units,
            () => _lots.SingleOrDefault(x => x.DateUnitsEnteredStorage == dateUnitsEnteredStorage
                && x.DateUnitsBecameUnfulfillable == dateUnitsBecameUnfulfillable
                && x.UnfulfillableCategory == unfulfillableCategory),
            () => new Lot(this, units, unfulfillableCategory));
    }

    public void TurnStranded()
    {
        IsStranded = true;

        foreach (var lot in FulfillableLots.Where(x => x.HasUnitsInStock))
        {
            lot.TurnStranded();
        }
    }

    public void ConfirmStrandingResolved()
    {
        IsStranded = false;

        foreach (var lot in StrandedLots.Where(x => x.HasUnitsInStock))
        {
            lot.ConfirmStrandingResolved();
        }
    }

    public void RemoveEmptyLots()
    {
        _lots.RemoveAll(x => !x.HasAnyUnits);
    }

    public void UpdateHasPickupsInProgress(bool hasPickupsInProgress)
    {
        HasPickupsInProgress = hasPickupsInProgress;
    }
    #endregion

    #region Helpers
    private Lot AddLotOrIncreaseLotUnits(uint units, Func<Lot?> findExisingLot, Func<Lot> createNewLot)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units), "Units cannot be 0.");
        }
        if (units > RemainingCapacity)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var existingLot = findExisingLot();
        if (existingLot != null)
        {
            existingLot.AdjustUnits((int)units);
            return existingLot;
        }
        else
        {
            var lot = createNewLot();
            _lots.Add(lot);
            return lot;
        }
    }

    private IEnumerable<Lot> GetLotsFifoForStockDemand(uint requiredUnits, Func<Lot, bool> filter)
    {
        if (requiredUnits == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredUnits), "Required units is zero.");
        }
        var lotsOldToNew = _lots.Where(x => filter(x))
            .OrderBy(x =>
            {
                if (x.IsUnitsFulfillable)
                    return x.DateUnitsEnteredStorage;
                if (x.IsUnitsUnfulfillable)
                    return x.DateUnitsBecameUnfulfillable;
                else
                    return x.DateUnitsBecameStranded;
            });
        var satisfactoryLots = new List<Lot>();
        foreach (var lot in lotsOldToNew)
        {
            var units = requiredUnits >= lot.UnitsInStock
                ? lot.UnitsInStock : requiredUnits;
            satisfactoryLots.Add(lot);
            requiredUnits -= units;
            if (requiredUnits == 0)
                break;
        }

        if (requiredUnits > 0)
        {
            throw new NotEnoughUnitsException();
        }
        return satisfactoryLots;
    }
    #endregion
}