namespace Bazaar.Contracting.Web.Messages;

public class ContractQuery
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ContractQuery(Contract contract)
    {
        Id = contract.Id;
        PartnerId = contract.PartnerId;
        SellingPlanId = contract.SellingPlanId;
        StartDate = contract.StartDate;
        EndDate = contract.EndDate;
    }
}