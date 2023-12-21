namespace Bazaar.FbbInventory.Domain.Entities;

public class Lot
{
    public int Id { get; private set; }
    public string LotNumber { get; private set; }
    public DateTime DateUnitsEnteredStorage { get; private set; }
    public DateTime? DateUnitsBecameStranded { get; private set; }
    public DateTime? DateUnitsBecameUnfulfillable { get; private set; }
    public UnfulfillableCategory? UnfulfillableCategory { get; private set; }
    public uint UnitsInStock { get; private set; }
    public uint UnitsInRemoval { get; private set; }
    public ProductInventory ProductInventory { get; private set; }
    public int ProductInventoryId { get; private set; }

    public uint TotalUnits => UnitsInStock + UnitsInRemoval;
    public bool IsUnitsStranded => DateUnitsBecameStranded != null;
    public bool IsUnitsUnfulfillable => DateUnitsBecameUnfulfillable != null;
    public bool HasUnitsInStock => UnitsInStock > 0;
    public bool HasAnyUnits => TotalUnits > 0;
    public bool IsUnfulfillableBeyondPolicyDuration =>
        DateUnitsBecameUnfulfillable + StoragePolicy.MaximumUnfulfillableDuration <= DateTime.Now.Date;

    #region Constructors
    /// <summary>
    /// Constructs fulfillable lot with the specified date that units entered storage and optionally
    /// the date that they became stranded, which, if specified, will result in a lot of stranded units. 
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="units"></param>
    /// <param name="dateUnitsEnteredStorage"></param>
    /// <param name="dateUnitsBecameStranded"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Lot(
        ProductInventory inventory, uint units,
        DateTime dateUnitsEnteredStorage, DateTime? dateUnitsBecameStranded = null)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (dateUnitsEnteredStorage.Date > DateTime.Now.Date)
        {
            throw new ArgumentOutOfRangeException(nameof(dateUnitsEnteredStorage),
                "Date units entered storage cannot be in the future.");
        }
        if (dateUnitsBecameStranded < dateUnitsEnteredStorage)
        {
            throw new ArgumentException(
                "Date units went stranded cannot be before date units entered storage.");
        }

        DateUnitsEnteredStorage = dateUnitsEnteredStorage;
        DateUnitsBecameStranded = dateUnitsBecameStranded;
        UnitsInStock = units;
        ProductInventory = inventory;
        ProductInventoryId = inventory.Id;
    }

    /// <summary>
    /// Constructs fulfillable lot with <see cref="DateUnitsEnteredStorage"/> set to current date.
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="units"></param>
    public Lot(
        ProductInventory inventory, uint units)
        : this(inventory, units, DateTime.Now.Date)
    {

    }

    /// <summary>
    /// Constructs unfulfillable lot with specified date units entered storage, 
    /// date units became unfulfillable and unfulfillable category.
    /// </summary>
    /// <param name="inventory"></param>
    /// <param name="units"></param>
    /// <param name="dateUnitsEnteredStorage"></param>
    /// <param name="dateUnitsBecameUnfulfillable"></param>
    /// <param name="unfulfillableCategory"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public Lot(
        ProductInventory inventory, uint units, DateTime dateUnitsEnteredStorage,
        DateTime dateUnitsBecameUnfulfillable, UnfulfillableCategory unfulfillableCategory)
        : this(inventory, units, dateUnitsEnteredStorage)
    {
        if (dateUnitsBecameUnfulfillable.Date > DateTime.Now.Date)
        {
            throw new ArgumentOutOfRangeException(nameof(dateUnitsEnteredStorage),
                "Date units went unfulfillable cannot be in the future.");
        }
        if (dateUnitsBecameUnfulfillable < DateUnitsEnteredStorage)
        {
            throw new ArgumentException(
                "Date units went unfulfillable cannot be before date units entered storage.");
        }
        if (!Enum.IsDefined(typeof(UnfulfillableCategory), unfulfillableCategory))
        {
            throw new ArgumentOutOfRangeException(nameof(unfulfillableCategory),
                "Unfulfillable category does not exist.");
        }

        DateUnitsBecameUnfulfillable = dateUnitsBecameUnfulfillable;
        UnfulfillableCategory = unfulfillableCategory;
    }

    public Lot(
        ProductInventory inventory, uint units, UnfulfillableCategory unfulfillableCategory)
        : this(inventory, units, DateTime.Now.Date, DateTime.Now.Date, unfulfillableCategory)
    {

    }

    // EF read constructor
    private Lot() { }
    #endregion

    public void IncreaseUnits(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (units > ProductInventory.RemainingCapacity)
        {
            throw new ExceedingMaxStockThresholdException();
        }

        UnitsInStock += units;
    }

    public void ReduceUnits(uint units)
    {
        if (units == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(units),
                "Units cannot be 0.");
        }
        if (units > UnitsInStock)
        {
            throw new NotEnoughUnitsException("Lot does not have enough units to reduce.");
        }

        UnitsInStock -= units;
    }

    public void SendUnitsForRemoval(uint units)
    {
        ReduceUnits(units);
        UnitsInRemoval += units;
    }

    public void RestoreUnitsFromRemoval(uint units)
    {
        if (units > UnitsInRemoval)
        {
            throw new NotEnoughUnitsException(
                "Lot units in removal are fewer than units to restore.");
        }
        UnitsInRemoval -= units;
        IncreaseUnits(units);
    }

    public void ConfirmUnitsRemoved(uint units)
    {
        if (units > UnitsInRemoval)
        {
            throw new NotEnoughUnitsException(
                "Not enough units in removal to confirm removed.");
        }
        UnitsInRemoval -= units;
    }

    public void TurnStranded()
    {
        if (IsUnitsUnfulfillable)
        {
            throw new InvalidOperationException(
                "Unfulfillable units cannot be rendered stranded.");
        }
        DateUnitsBecameStranded = DateTime.Now.Date;
    }

    public void ConfirmStrandingResolved()
    {
        if (!IsUnitsStranded)
        {
            throw new InvalidOperationException("Lot units are not stranded.");
        }
        DateUnitsBecameStranded = null;
    }
}

public enum UnfulfillableCategory
{
    Defective,
    WarehouseDamaged,
    CustomerDamaged,
}