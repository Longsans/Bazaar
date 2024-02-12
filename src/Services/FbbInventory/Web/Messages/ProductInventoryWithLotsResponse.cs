namespace Bazaar.FbbInventory.Web.Messages;

public record ProductInventoryWithLotsResponse : ProductInventoryResponse
{
    public IEnumerable<LotResponse> FulfillableLots { get; }
    public IEnumerable<LotResponse> UnfulfillableLots { get; }
    public IEnumerable<LotResponse> StrandedLots { get; }

    public ProductInventoryWithLotsResponse(ProductInventory inventory) : base(inventory)
    {
        FulfillableLots = inventory.FulfillableLots.Select(x => new LotResponse(x));
        UnfulfillableLots = inventory.UnfulfillableLots.Select(x => new LotResponse(x));
        StrandedLots = inventory.StrandedLots.Select(x => new LotResponse(x));
    }
}
