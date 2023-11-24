namespace Bazaar.FbbInventory.Application.DTOs;

public record StockUnitsRemovalDto(string ProductId, uint FulfillableUnits, uint UnfulfillableUnits)
{
    public string ProductId { get; set; } = ProductId;
    public uint FulfillableUnits { get; set; } = FulfillableUnits;
    public uint UnfulfillableUnits { get; set; } = UnfulfillableUnits;
}
