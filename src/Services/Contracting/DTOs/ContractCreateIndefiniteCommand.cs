namespace Bazaar.Contracting.DTOs;

public class ContractCreateIndefiniteCommand
{
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }

    public Contract ToContractInfo() => new()
    {
        SellingPlanId = SellingPlanId,
        StartDate = StartDate,
    };
}
