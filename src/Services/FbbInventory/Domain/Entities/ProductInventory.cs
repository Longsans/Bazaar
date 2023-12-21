namespace Bazaar.FbbInventory.Domain.Entities;

public class ProductInventory
{
    public int Id { get; private set; }
    public string ProductId { get; private set; }

    private readonly List<Lot> _lots;
    public IReadOnlyCollection<Lot> Lots => _lots.AsReadOnly();
    public IReadOnlyCollection<Lot> FulfillableLots
        => _lots.Where(x => !x.IsUnitsUnfulfillable && !x.IsUnitsStranded).ToList().AsReadOnly();
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
        string productId, uint fulfillableUnits,
        uint defectiveUnits, uint warehouseDamagedUnits,
        uint restockThreshold, uint maxStockThreshold, int sellerInventoryId)
    {
        var totalUnits = fulfillableUnits + defectiveUnits + warehouseDamagedUnits;
        if (totalUnits == 0)
        {
            throw new ArgumentOutOfRangeException(
                "The total number of units in product inventory cannot be 0.", null as Exception);
        }
        if (restockThreshold > maxStockThreshold
            || fulfillableUnits + defectiveUnits + warehouseDamagedUnits > maxStockThreshold)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        ProductId = productId;
        RestockThreshold = restockThreshold;
        MaxStockThreshold = maxStockThreshold;
        SellerInventoryId = sellerInventoryId;

        _lots = new();
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
    /// <summary>
    /// Issues fulfillable/stranded stock (both are referred to as <em>good stock</em>) and 
    /// unfulfillable stock starting with older stock and ending with newer stock until satisfies
    /// <paramref name="goodUnits"/> and <paramref name="unfulfillableUnits"/>, respectively.
    /// Fulfillable stock is issued by date entered storage.
    /// Unfulfillable stock is issued by date they became unfulfillable.
    /// Stranded stock is issued by date they became stranded.
    /// </summary>
    /// <param name="goodUnits"></param>
    /// <param name="unfulfillableUnits"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotEnoughUnitsException"></exception>
    public StockIssue IssueStockByDateOldToNew(
        uint goodUnits, uint unfulfillableUnits, StockIssueReason issueReason)
    {
        if (goodUnits + unfulfillableUnits == 0)
        {
            throw new ArgumentException("Total units to issue cannot be 0.");
        }
        if (!IsStranded && goodUnits > FulfillableUnits)
        {
            throw new NotEnoughUnitsException("Not enough fulfillable stock.");
        }
        if (IsStranded && goodUnits > StrandedUnits)
        {
            throw new NotEnoughUnitsException("Not enough stranded stock.");
        }
        if (unfulfillableUnits > UnfulfillableUnits)
        {
            throw new NotEnoughUnitsException("Not enough unfulfillable stock.");
        }

        StockIssue? issue = null;
        if (goodUnits > 0)
        {
            issue = IssueStockByDateCreatedOldToNew(
                IsStranded ? StrandedLots : FulfillableLots, goodUnits, issueReason);
        }
        if (unfulfillableUnits > 0)
        {
            var unfulIssue = IssueStockByDateCreatedOldToNew(
                UnfulfillableLots, unfulfillableUnits, issueReason);
            if (issue == null)
            {
                issue = unfulIssue;
            }
            else
            {
                issue.AddItems(unfulIssue.Items);
            }
        }
        RemoveEmptyLots();
        return issue!;
    }

    public void ReceiveFulfillableStock(uint units)
    {
        if (IsStranded)
        {
            throw new StockStrandedException();
        }
        AddFulfillableStock(DateTime.Now.Date, units);
    }

    public void ReceiveUnfulfillableStock(UnfulfillableCategory unfulfillableCategory, uint units)
    {
        AddUnfulfillableStock(DateTime.Now.Date, DateTime.Now.Date, unfulfillableCategory, units);
    }

    public void ReceiveStrandedStock(uint units)
    {
        if (!IsStranded)
        {
            throw new StockNotStrandedException();
        }
        AddStrandedStock(DateTime.Now.Date, DateTime.Now.Date, units);
    }

    public void AdjustFulfillableStock(DateTime dateEnteredStorage, int units)
    {
        uint unsignedUnits = (uint)Math.Abs(units);
        if (units > 0)
        {
            if (IsStranded)
            {
                throw new StockStrandedException("Product stock is stranded. Cannot adjust fulfillable stock." +
                    "Adjust stranded stock instead.");
            }
            AddFulfillableStock(dateEnteredStorage, unsignedUnits);
        }
        else
        {
            ReduceFulfillableStock(dateEnteredStorage, unsignedUnits);
            RemoveEmptyLots();
        }
    }

    public void AdjustUnfulfillableStock(DateTime dateStockEnteredStorage,
        DateTime dateStockBecameUnfulfillable, UnfulfillableCategory unfulfillableCategory, int units)
    {
        uint unsignedUnits = (uint)Math.Abs(units);
        if (units > 0)
        {
            AddUnfulfillableStock(dateStockEnteredStorage,
                dateStockBecameUnfulfillable, unfulfillableCategory, unsignedUnits);
        }
        else
        {
            ReduceUnfulfillableStock(dateStockEnteredStorage,
                dateStockBecameUnfulfillable, unfulfillableCategory, unsignedUnits);
            RemoveEmptyLots();
        }
    }

    public void AdjustStrandedStock(DateTime dateStockEnteredStorage, DateTime dateStockBecameStranded, int units)
    {
        uint unsignedUnits = (uint)Math.Abs(units);
        if (units > 0)
        {
            if (!IsStranded)
            {
                throw new StockNotStrandedException(
                    "Product stock is not stranded. Adjust fulfillable stock instead.");
            }
            AddStrandedStock(dateStockEnteredStorage, dateStockBecameStranded, unsignedUnits);
        }
        else
        {
            ReduceStrandedStock(dateStockEnteredStorage, dateStockBecameStranded, unsignedUnits);
            RemoveEmptyLots();
        }
    }

    public void TurnStranded()
    {
        if (IsStranded || FulfillableUnits == 0)
        {
            return;
        }
        IsStranded = true;

        foreach (var lot in FulfillableLots.Where(x => x.HasUnitsInStock))
        {
            lot.TurnStranded();
        }
    }

    public void ConfirmStrandingResolved()
    {
        if (!IsStranded)
        {
            return;
        }
        var strandedLots = StrandedLots.Where(x => x.HasUnitsInStock).ToList();
        if (!strandedLots.Any())
        {
            return;
        }
        IsStranded = false;

        foreach (var lot in strandedLots)
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
    private Lot AddFulfillableStock(DateTime dateEnteredStorage, uint units)
    {
        return IncreaseLotUnitsOrAddLot(units,
            () => FindFulfillableLot(dateEnteredStorage),
            () => new Lot(this, units, dateEnteredStorage));
    }

    private Lot AddUnfulfillableStock(DateTime dateEnteredStorage, DateTime dateUnfulfillableSince,
        UnfulfillableCategory unfulfillableCategory, uint units)
    {
        return IncreaseLotUnitsOrAddLot(units,
            () => FindUnfulfillableLot(dateEnteredStorage, dateUnfulfillableSince, unfulfillableCategory),
            () => new Lot(this, units, dateEnteredStorage, dateUnfulfillableSince, unfulfillableCategory));
    }

    private Lot AddStrandedStock(DateTime dateEnteredStorage, DateTime dateWentStranded, uint units)
    {
        return IncreaseLotUnitsOrAddLot(units,
            () => FindStrandedLot(dateEnteredStorage, dateWentStranded),
            () => new Lot(this, units, dateEnteredStorage, dateWentStranded));
    }

    private Lot ReduceFulfillableStock(DateTime dateEnteredStorage, uint units)
    {
        var lot = FindFulfillableLot(dateEnteredStorage)
            ?? throw new KeyNotFoundException(
                "No fulfillable stock entered storage on specified date.");
        lot.ReduceUnits(units);
        return lot;
    }

    private Lot ReduceUnfulfillableStock(DateTime dateEnteredStorage,
        DateTime dateUnfulfillableSince, UnfulfillableCategory unfulfillableCategory, uint units)
    {
        var lot = FindUnfulfillableLot(dateEnteredStorage,
            dateUnfulfillableSince, unfulfillableCategory)
            ?? throw new KeyNotFoundException(
                "No unfulfillable stock with specified enter storage date, " +
                "unfulfillable date and unfulfillable category.");
        lot.ReduceUnits(units);
        return lot;
    }

    private Lot ReduceStrandedStock(DateTime dateEnteredStorage, DateTime dateWentStranded, uint units)
    {
        var lot = FindStrandedLot(dateEnteredStorage, dateWentStranded)
            ?? throw new KeyNotFoundException(
                "No stranded stock with specified date entered storage and date went stranded.");
        lot.ReduceUnits(units);
        return lot;
    }

    private Lot? FindFulfillableLot(DateTime dateEnteredStorage)
    {
        return _lots.SingleOrDefault(x =>
            !x.IsUnitsUnfulfillable && !x.IsUnitsStranded
            && x.DateUnitsEnteredStorage == dateEnteredStorage);
    }

    private Lot? FindUnfulfillableLot(DateTime dateEnteredStorage,
        DateTime dateUnfulfillableSince, UnfulfillableCategory unfulfillableCategory)
    {
        return _lots.SingleOrDefault(x =>
            x.DateUnitsEnteredStorage == dateEnteredStorage
            && x.DateUnitsBecameUnfulfillable == dateUnfulfillableSince
            && x.UnfulfillableCategory == unfulfillableCategory);
    }

    private Lot? FindStrandedLot(DateTime dateEnteredStorage, DateTime dateWentStranded)
    {
        return _lots.SingleOrDefault(x =>
            x.DateUnitsEnteredStorage == dateEnteredStorage
            && x.DateUnitsBecameStranded == dateWentStranded);
    }

    private Lot IncreaseLotUnitsOrAddLot(uint units, Func<Lot?> findExisingLot, Func<Lot> createNewLot)
    {
        if (units == 0)
        {
            throw new ArgumentException("Units cannot be 0.");
        }
        if (units > RemainingCapacity)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        var existingLot = findExisingLot();
        if (existingLot != null)
        {
            existingLot.IncreaseUnits(units);
            return existingLot;
        }
        else
        {
            var lot = createNewLot();
            _lots.Add(lot);
            return lot;
        }
    }

    private StockIssue IssueStockByDateCreatedOldToNew(
        IEnumerable<Lot> lots, uint units, StockIssueReason issueReason)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(units), "Number of stock units to reduce cannot be 0.");
        }

        var stockIssueItems = new List<StockIssueItem>();
        var lotsByEventDateOldToNew = lots.Where(x => x.HasUnitsInStock)
            .OrderBy(x =>
            {
                if (x.IsUnitsUnfulfillable)
                    return x.DateUnitsBecameUnfulfillable;
                else if (x.IsUnitsStranded)
                    return x.DateUnitsBecameStranded;
                else return x.DateUnitsEnteredStorage;
            });
        foreach (var lot in lotsByEventDateOldToNew)
        {
            var removeUnits = units >= lot.UnitsInStock ? lot.UnitsInStock : units;
            var issueItem = new StockIssueItem(ProductId, lot.LotNumber, removeUnits);
            switch (issueReason)
            {
                case StockIssueReason.Sale:
                    lot.ReduceUnits(removeUnits);
                    break;
                case StockIssueReason.Return:
                case StockIssueReason.Disposal:
                    lot.SendUnitsForRemoval(removeUnits);
                    break;
                default:
                    throw new ArgumentException("Issue reason does not exist.");
            }

            stockIssueItems.Add(issueItem);
            units -= removeUnits;
            if (units == 0)
            {
                break;
            }
        }
        return new StockIssue(stockIssueItems, issueReason);
    }
    #endregion
}