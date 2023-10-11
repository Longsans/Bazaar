namespace Bazaar.Contracting.Application.DTOs;

public class ContractDto
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ContractDto() { }

    public ContractDto(Contract contract)
    {
        Id = contract.Id;
        PartnerId = contract.PartnerId;
        SellingPlanId = contract.SellingPlanId;
        StartDate = contract.StartDate;
        EndDate = contract.EndDate;
    }
}
