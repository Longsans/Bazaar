namespace Bazaar.Contracting.Web.DTOs;

public class CreateFixedPeriodContractRequest
{
    public int SellingPlanId { get; set; }
    public DateTime EndDate { get; set; }
}
