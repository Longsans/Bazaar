namespace WebSellerUI.Model;

public class Contract
{
    public int Id { get; set; }
    public int PartnerId { get; set; }
    public int SellingPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
