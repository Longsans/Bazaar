namespace Bazaar.Contracting.DTOs;

public class ContractCreateIndefiniteCommand
{
    public int SellingPlanId { get; set; }

    public Contract ToContractInfo() => new()
    {
        SellingPlanId = SellingPlanId,
        StartDate = DateTime.Now,
    };
}
