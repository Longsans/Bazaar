namespace Bazaar.Contracting.DTOs;

public class ContractCreateFixedPeriodCommand
{
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Contract ToContractInfo() => new()
    {
        SellingPlanId = SellingPlanId,
        StartDate = StartDate,
        EndDate = EndDate
    };
}
