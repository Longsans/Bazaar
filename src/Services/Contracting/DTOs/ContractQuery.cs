using Contracting.Model;

namespace Contracting.Dto;

public class ContractQuery
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int SellingPlanId { get; set; }
    public SellingPlan SellingPlan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ContractQuery(Contract contract)
    {
        Id = contract.Id;
        PartnerId = contract.PartnerId;
        SellingPlanId = contract.SellingPlanId;
        SellingPlan = contract.SellingPlan;
        StartDate = contract.StartDate;
        EndDate = contract.EndDate;
    }
}