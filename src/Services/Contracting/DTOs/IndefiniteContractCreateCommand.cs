namespace Bazaar.Contracting.DTOs;

public class IndefiniteContractCreateCommand
{
    public int SellingPlanId { get; set; }

    public Contract ToContract(int partnerId) => new()
    {
        PartnerId = partnerId,
        SellingPlanId = SellingPlanId,
        StartDate = DateTime.Now,
    };
}
