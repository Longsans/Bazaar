namespace Bazaar.Contracting.Web.Messages;

public class CreateFixedPeriodContractRequest
{
    public int SellingPlanId { get; set; }
    public DateTime EndDate { get; set; }
}
