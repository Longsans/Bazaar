namespace Bazaar.Contracting.Domain.Entities;

public class Contract
{
    public int Id { get; private set; }

    public Partner Partner { get; private set; }
    public int PartnerId { get; private set; }

    public SellingPlan SellingPlan { get; private set; }
    public int SellingPlanId { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    // For creating new contracts
    public Contract(int partnerId, int sellingPlanId, DateTime? endDate)
    {
        if (endDate != null && endDate < DateTime.Now.Date)
            throw new EndDateBeforeCurrentDateException();

        PartnerId = partnerId;
        SellingPlanId = sellingPlanId;
        StartDate = DateTime.Now.Date;
        EndDate = endDate?.Date;
    }

    public void End()
    {
        if (EndDate <= DateTime.Now.Date)
            throw new ContractEndedException();

        EndDate = DateTime.Now.Date;
    }

    public void Extend(DateTime extendedEndDate)
    {
        if (EndDate == null)
            throw new ExtendIndefiniteContractException();

        if (EndDate < DateTime.Now.Date)
            throw new ContractEndedException();

        if (extendedEndDate <= EndDate)
            throw new ExtendedEndDateNotAfterOriginalEndDateException();

        EndDate = extendedEndDate.Date;
    }
}