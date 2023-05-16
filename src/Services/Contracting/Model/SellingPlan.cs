namespace Contracting.Model;

public class SellingPlan
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal MonthlyFee { get; set; }
    public decimal PerSaleFee { get; set; }
    public float RegularPerSaleFeePercent { get; set; }
}