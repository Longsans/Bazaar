namespace Bazaar.FbbInventory.Domain.Entities;

public class UnfulfillableLot : Lot
{
    public UnfulfillableCategory UnfulfillableCategory { get; private set; }
    public DateTime DateUnfulfillableSince { get; private set; }

    public bool IsUnfulfillableBeyondPolicyDuration
        => DateUnfulfillableSince + StoragePolicy.MaximumUnfulfillableDuration <= DateTime.Now.Date;

    public UnfulfillableLot(ProductInventory inventory,
        uint unitsInStock, UnfulfillableCategory unfulfillableCategory)
        : base(inventory, unitsInStock)
    {
        UnfulfillableCategory = unfulfillableCategory;
        DateUnfulfillableSince = DateTime.Now.Date;
    }

    private UnfulfillableLot() { }
}

public enum UnfulfillableCategory
{
    Defective,
    WarehouseDamaged,
    CustomerDamaged,
    Stranded
}