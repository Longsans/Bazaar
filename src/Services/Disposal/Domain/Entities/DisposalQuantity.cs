namespace Bazaar.Disposal.Domain.Entities;

public class DisposalQuantity(string lotNumber, uint unitsToDispose, string inventoryOwnerId)
{
    public int Id { get; private set; }
    public string LotNumber { get; private set; } = lotNumber;
    public uint UnitsToDispose { get; private set; } = unitsToDispose;
    public string InventoryOwnerId { get; private set; } = inventoryOwnerId;
    public DisposalOrder DisposalOrder { get; private set; }
    public int DisposalOrderId { get; private set; }
}
