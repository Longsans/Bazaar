namespace Bazaar.Transport.Domain.Specifications;

public static class PickupPredicates
{
    public static Func<InventoryPickup, string, bool> ContainsProduct
        => (x, pid) => x.ProductStocks.Any(x => x.ProductId == pid);
}
