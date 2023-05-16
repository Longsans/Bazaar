namespace Contracting.Model;

public class Contract
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public Partner Partner { get; set; }
    public int SellingPlanId { get; set; }
    public SellingPlan SellingPlan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}