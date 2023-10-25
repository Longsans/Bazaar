namespace Bazaar.Contracting.Web.Messages;

public class ContractResponse
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ContractResponse(Contract contract)
    {
        Id = contract.Id;
        ClientId = contract.ClientId;
        SellingPlanId = contract.SellingPlanId;
        StartDate = contract.StartDate;
        EndDate = contract.EndDate;
    }
}