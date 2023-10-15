namespace Bazaar.Contracting.Domain.Entities;

public class Contract
{
    public int Id { get; set; }

    public Partner Partner { get; private set; }
    public int PartnerId { get; private set; }

    public SellingPlan SellingPlan { get; private set; }
    public int SellingPlanId { get; private set; }

    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public Contract(int partnerId, int sellingPlanId, DateTime startDate, DateTime? endDate)
    {
        PartnerId = partnerId;
        SellingPlanId = sellingPlanId;
        StartDate = startDate;
        EndDate = endDate;
    }

    public Contract(Partner partner, SellingPlan sellingPlan, DateTime startDate, DateTime? endDate)
    {
        Partner = partner;
        SellingPlan = sellingPlan;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateEndDate(DateTime endDate)
    {
        EndDate = endDate.Date > DateTime.Now.Date
            ? endDate.Date
            : throw new ArgumentException(
                "New end date must be after current date", nameof(endDate));
    }
}