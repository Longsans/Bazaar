namespace Bazaar.Contracting.DTOs;

public class FixedPeriodContractCreateCommand
{
    public int SellingPlanId { get; set; }
    public DateTime EndDate { get; set; }

    public Contract ToContract(int partnerId) => new()
    {
        PartnerId = partnerId,
        SellingPlanId = SellingPlanId,
        EndDate = EndDate
    };
}
