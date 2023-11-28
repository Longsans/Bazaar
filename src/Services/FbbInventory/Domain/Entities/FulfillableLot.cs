namespace Bazaar.FbbInventory.Domain.Entities;

public class FulfillableLot : Lot
{
    public DateTime DateEnteredStorage { get; private set; }

    public FulfillableLot(
        ProductInventory inventory, uint unitsInStock) : base(inventory, unitsInStock)
    {
        DateEnteredStorage = DateTime.Now.Date;
    }

    private FulfillableLot() { }
}
